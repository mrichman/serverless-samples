package main

import (
	"context"
	"encoding/json"
	"os"
	"strings"

	"github.com/aws/aws-lambda-go/events"
	"github.com/aws/aws-lambda-go/lambda"
	"github.com/aws/aws-sdk-go-v2/aws"
	"github.com/aws/aws-sdk-go-v2/config"
	"github.com/aws/aws-sdk-go-v2/feature/dynamodb/attributevalue"
	"github.com/aws/aws-sdk-go-v2/service/dynamodb"
	"github.com/aws/aws-xray-sdk-go/instrumentation/awsv2"
	"github.com/rs/zerolog"
	"github.com/rs/zerolog/log"
	"github.com/segmentio/ksuid"
)

type dependency struct {
	dynamodBClient dynamoDBClient
	table          string
}

// Record represents one record in the DynamoDB table
type Record struct {
	ID   string `dynamodbav:"id"`
	Body string
}

func (d *dependency) handler(ctx context.Context, request events.APIGatewayProxyRequest) (events.APIGatewayProxyResponse, error) {

	data, _ := json.Marshal(request)
	log.Debug().Msgf("Event : %s\n", string(data))
	log.Debug().Msg(strings.Join(os.Environ(), "\n"))

	if request.Body == "" {
		return events.APIGatewayProxyResponse{
			StatusCode: 400,
			Body:       "Missing body",
		}, nil
	}

	// Create a new record from the request
	r := Record{
		ID:   ksuid.New().String(),
		Body: request.Body,
	}

	// Marshal that record into a DynamoDB AttributeMap
	av, err := attributevalue.MarshalMap(r)
	if err != nil {
		log.Error().Err(err).Send()
		return events.APIGatewayProxyResponse{
			StatusCode: 500,
		}, err
	}

	log.Info().Msgf("Putting record into DynamoDB table %s", d.table)

	_, err = d.dynamodBClient.PutItem(ctx, &dynamodb.PutItemInput{
		TableName: aws.String(d.table),
		Item:      av,
	})

	if err != nil {
		log.Error().Err(err).Send()
		return events.APIGatewayProxyResponse{
			StatusCode: 500,
		}, err
	}

	return events.APIGatewayProxyResponse{
		StatusCode: 200,
	}, nil
}

func main() {

	// UNIX Time is faster and smaller than most timestamps
	zerolog.TimeFieldFormat = zerolog.TimeFormatUnix

	log.Info().Msg("In main")

	cfg, err := config.LoadDefaultConfig(context.Background())
	if err != nil {
		log.Error().Err(err).Send()
	}

	awsv2.AWSV2Instrumentor(&cfg.APIOptions)

	d := &dependency{dynamodBClient: dynamodb.NewFromConfig(cfg), table: os.Getenv("DYNAMODB_TABLE")}

	log.Info().Msg("Starting Lambda handler")
	lambda.Start(d.handler)
}
