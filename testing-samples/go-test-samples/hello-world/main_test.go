package main

import (
	"context"
	"hello-world/mocks"
	"testing"

	"github.com/aws/aws-lambda-go/events"
	"github.com/aws/aws-sdk-go-v2/service/dynamodb"
	"github.com/aws/aws-xray-sdk-go/xray"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/mock"
)

func TestLambdaHandler(t *testing.T) {
	t.Run("Successful request", func(t *testing.T) {
		m := mocks.DynamoDBClient{}
		m.On("PutItem", mock.Anything, mock.Anything).Return(&dynamodb.PutItemOutput{}, nil)
		m.On("GetItem", mock.Anything, mock.Anything).Return(&dynamodb.GetItemOutput{}, nil)

		// set the principal in the context
		ctx, seg := xray.BeginSegment(context.Background(), "Test")
		defer seg.Close(nil)

		d := dependency{dynamodBClient: &m, table: "test_table"}

		resp, err := d.handler(ctx, events.APIGatewayProxyRequest{
			Body: `{"message": "Hello World"}`,
		})
		if err != nil {
			t.Errorf("Unexpected error: %v", err)
		}

		assert.Equal(t, resp.StatusCode, 200, "response should be 200")
	})
}
