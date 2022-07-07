import { APIGatewayProxyEvent, APIGatewayProxyResult } from 'aws-lambda';
import { S3Client, ListBucketsCommand } from '@aws-sdk/client-s3';
const region = process.env.AWS_REGION ?? 'us-east-1';
const s3Client = new S3Client({ region: region });
export const lambdaHandler = async (event: APIGatewayProxyEvent): Promise<APIGatewayProxyResult> => {
    try {
        const listBucketsOutput = await s3Client.send(new ListBucketsCommand({}));
        let bucketList = '';
        if (listBucketsOutput.Buckets) {
            bucketList = listBucketsOutput.Buckets?.map((bucket) => bucket.Name).join(' | ');
        }
        console.log('Hello logfile!');
        return {
            statusCode: 200,
            body: bucketList,
        };
    } catch (error) {
        console.error('Error', error);
        return {
            statusCode: 500,
            body: '',
        };
    }
};
