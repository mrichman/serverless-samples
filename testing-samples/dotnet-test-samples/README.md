**THIS PROJECT IS A WORK IN PROGRESS**

**THESE MATERIALS ARE EXAMPLES ONLY AND NOT MEANT FOR USE IN PRODUCTION ENVIRONMENTS**

# .NET Test Samples
This project contains automated test sample code samples for serverless applications. The project uses the [AWS Serverless Application Model](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/what-is-sam.html) (SAM) CLI for configuration, testing and deployment. 

- [Project contents](#project-contents)
- [Prerequesites](#prerequesites)
- [Build and deploy with the SAM CLI](#build-and-deploy-with-the-sam-cli)
- [Working with events](#working-with-events)
- [Working with local emulators](#working-with-local-emulators)
  - [Use the SAM Lambda emulator](#use-the-sam-lambda-emulator)
  - [Use the SAM API Gateway emulator](#use-the-sam-api-gateway-emulator)
- [Run a unit test using a mock framework](#run-a-unit-test-using-a-mock-framework)
- [Run an integration test against cloud resources](#run-integration-tests-against-cloud-resources)
- [Invoke a Lambda function in the cloud](#invoke-a-lambda-function-in-the-cloud)
- [Fetch, tail, and filter Lambda function logs locally](#fetch-tail-and-filter-lambda-function-logs-locally)
- [Use SAM Accerate to speed up feedback cycles](#use-sam-accerate-to-speed-up-feedback-cycles)
- [Use CDK Watch to speed up feedback cycles](#use-cdk-watch-to-speed-up-feedback-cycles)
- [Perform a load test](#perform-a-load-test)
- [Implement application tracing](#implement-application-tracing)
- [Cleanup](#cleanup)
- [Additional resources](#additional-resources)

## Project contents
This application creates several AWS resources, including a Lambda function and an API Gateway. These resources are defined in the `template.yaml` file in this project. This project includes the following files and folders:

- src - Code for the application's Lambda function.
- events - synthetic events that you can use to invoke the function.
- tests - Unit and integration tests for the application code. 
- template.yaml - A template that defines the application's AWS resources.

## Prerequesites
The SAM CLI is an extension of the AWS CLI that adds functionality for building and testing serverless applications. It contains features for building your application locally, deploying it to AWS, and emulating AWS services locally to support automated unit tests.  

To use the SAM CLI, you need the following tools.

* SAM CLI - [Install the SAM CLI](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install.html)
* .NET 6 - [Install .NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* Docker - [Install Docker community edition](https://hub.docker.com/search/?type=edition&offering=community)

[[top]](#dotnet-test-samples)

## Build and deploy with the SAM CLI
Use the following command to build your application locally: 

```bash
# build your application locally using a container
dotnet-test-samples$ sam build
```
The SAM CLI installs runs a dotnet publish for each function defined in the template, creates a deployment package, and saves it in the `.aws-sam/build` folder. [Read the documentation](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-building.html).

Use the following command to deploy your application package to AWS: 

``` bash
# deploy your application to the AWS cloud 
dotnet-test-samples$ sam deploy --guided
```

After running this command you will receive a series of prompts:

* **Stack Name**: The name of the stack to deploy to CloudFormation. This should be unique to your account and region, and a good starting point would be something matching your project name. Use `dotnet-test-samples` as the stack name for this project.
* **AWS Region**: The AWS region you want to deploy your app to.
* **Confirm changes before deploy**: If set to yes, any change sets will be shown to you before execution for manual review. If set to no, the AWS SAM CLI will automatically deploy application changes.
* **Allow SAM CLI IAM role creation**: Many AWS SAM templates, including this example, create AWS IAM roles required for the AWS Lambda function(s) included to access AWS services. By default, these are scoped down to minimum required permissions. To deploy an AWS CloudFormation stack which creates or modifies IAM roles, the `CAPABILITY_IAM` value for `capabilities` must be provided. If permission isn't provided through this prompt, to deploy this example you must explicitly pass `--capabilities CAPABILITY_IAM` to the `sam deploy` command.
* **DotnetTestDemo may not have authorisation defined, Is this okay?**: If a Lambda function is defined with an API event that does not have authorisation defined the AWS SAM CLI will ask you to confirm that this is ok.
* **Save arguments to samconfig.toml**: If set to yes, your choices will be saved to a configuration file inside the project, so that in the future you can just re-run `sam deploy` without parameters to deploy changes to your application.

You can find your API Gateway Endpoint URL in the output values displayed after deployment. Take note of this URL for use in the logging section below. On subsequent deploys you can run `sam deploy` without the `--guided` flag. [Read the documentation](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-deploying.html).
[[top]](#python-test-samples)

## Working with events
Testing event driven architectures often requires working with synthetic events. Events are frequently defined as JSON documents. Synthetic events are test data that represent AWS events such as a requests from API Gateway or a messages from SQS. 

AWS Lambda always requires an event during invocation. A sample test event is included in the `events` folder in this project. SAM provides the capability of generating additional synthetic events for a variety of AWS services. [Read the documentation](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/sam-cli-command-reference-sam-local-generate-event.html).

Use the following command to learn more about generating synthetic events:
```bash
# generate a synthetic event
dotnet-test-samples$ sam local generate-event
```
[[top]](#dotnet-test-samples)

## Working with local emulators
Local emulation of AWS services offers a simple way to build and test cloud native applications using local resources. Local emulation can speed up the build and deploy cycle creating faster feedback loops for application developers. 

Local emulation has several limitations. Cloud services evolve rapidly, so local emulators are unlikely to have feature parity with their counterpart services in the cloud. Local emulators may not be able to provide an accurate representation of IAM permissions or service quotas. Local emulators do not exist for every AWS service.

SAM provides local emulation features for [AWS Lambda](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-using-invoke.html) and [Amazon API Gateway](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-using-start-api.html). AWS provides [Amazon DynamoDB Local](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.html) as well as [AWS Step Functions Local](https://docs.aws.amazon.com/step-functions/latest/dg/sfn-local.html). Third party vendors like [LocalStack](https://docs.localstack.cloud/overview/) may provide emulation for additional AWS services. 

This project demonstrates local emulation of Lambda and API Gateway with SAM.

[[top]](#dotnet-test-samples)

## Use the SAM Lambda emulator 
The SAM CLI can emulate a Lambda function inside a Docker container deployed to your local desktop. To use this feature, invoke the function with the `sam local invoke` command passing a synthetic event. Print statements log to standard out. [Read the documentation](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-using-invoke.html).

**TODO: Add detail Lambda emulator**

[[top]](#dotnet-test-samples)

## Use the SAM API Gateway emulator
**TODO: Add detail about SAM API Gateway emulator**

The SAM CLI reads the application template to determine the API's routes and the functions that they invoke. The `Events` property on each function's definition includes the route and method for each path.

```yaml
      Events:
        HelloWorld:
          Type: Api
          Properties:
            Path: /hello
            Method: get
```
[[top]](#python-test-samples)

## Run a unit test using a mock framework
Lambda functions frequently call other AWS or 3rd party services. Mock frameworks are useful to simulate service responses. Mock frameworks can speed the development process by enabling rapid feedback iterations. Mocks can be particularly useful for testing failure cases when testing these branches of logic are difficult to do in the cloud.

The project uses mocks to test the internal logic of a Lambda function.
The project uses the [Moq](https://github.com/moq/moq4) library to provide a mock implementation of the AmazonS3Client. This allows test cases to be easily configured to test the different possible responses from Amazon S3.

```bash

# run unit tests with mocks
dotnet-test-samples$ dotnet test .\tests\ServerlessTestSamples.UnitTest\
```

The constructor of the Lambda function class contains one public, functionless constructor and one internal constructor. The Lambda service requires a parameterless constructor. The internal constructor is included to allow mock implementations of services to be passed in at initialization. The internal constructor is made available to the ServerlessTestSamples.UnitTest project through the AssemblyInfo.cs file.

```c#
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("ServerlessTestSamples.UnitTest")]
```

[[top]](#dotnet-test-samples)

## Run integration tests against cloud resources
Integration tests run against deployed cloud resources. Since local unit tests cannot adequately test IAM permissions, our integration tests confirm that permissions are properly configured. Run integration tests against your deployed cloud resources with the following command:

```bash
# Set the environment variable AWS_SAM_STACK_NAME to the name of the stack you specified during deploy
dotnet-test-samples$ AWS_SAM_STACK_NAME=<stack-name> dotnet test .\tests\ServerlessTestSamples.IntegrationTest\
```
[[top]](#dotnet-test-samples)

## Invoke a Lambda function in the cloud
The `AWS CLI` enables you to invoke a Lambda function in the cloud.

```bash
# invoke a Lambda function in the cloud using the AWS CLI
aws lambda invoke --function-name dotnet-test-samples-DotnetTestDemo-hqVByFXNxqBC outfile.txt
```
[[top]](#dotnet-test-samples)

## Fetch, tail, and filter Lambda function logs locally
To simplify troubleshooting, SAM CLI has a command called `sam logs`. The `sam logs` command lets you fetch logs generated by your deployed Lambda function from the command line. In addition to printing the logs on the terminal, this command has several features to help you quickly find your bug.

`NOTE`: This command works for all AWS Lambda functions; not just the ones you deploy using SAM.

```bash
dotnet-test-samples$ sam logs -n DotnetTestDemo --stack-name dotnet-test-samples --tail
```

In a new terminal, curl the API Gateway and watch the log output.

```bash
dotnet-test-samples$ curl <API Gateway url>
```

You can find more information and examples about filtering Lambda function logs in the [SAM CLI Documentation](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-logging.html).

[[top]](#dotnet-test-samples)

## Use SAM Accerate to speed up feedback cycles
AWS SAM Accelerate is a set of features that reduces deployment latency and enable developers to test their code quickly against production AWS services in the cloud.
[Read the blog post](https://aws.amazon.com/blogs/compute/accelerating-serverless-development-with-aws-sam-accelerate/)

```bash
# synchronize local code with the cloud
dotnet-test-samples$ sam sync --watch --stack-name dotnet-test-samples
```
[[top]](#dotnet-test-samples)

## Use CDK Watch to speed up feedback cycles

[[top]](#python-test-samples)

## Perform a load test
Load tests should be executed in the cloud prior to any initial deployment to production environments. Load tests can be useful to discover performance bottlenecks and quota limits. Load tests should be scripted and repeatable. Load tests should simulate your application's expected peak load. 

[Artillery](https://www.artillery.io/) is used for load testing. Load tests scenarios are configured using a yaml configuration file. The configured load tests runs 100 requests / second for 10 minutes to our API endpoints.

To execute the load tests run either the PowerShell or bash scripts under the [load test directory](./loadtest).

```bash
cd loadtest
./run-load-test.sh
```

```powershell
cd loadtest
./run-load-test.ps1
```

[[top]](#python-test-samples)

## Implement application tracing
You can use AWS X-Ray to track user requests as they travel through your entire application. With X-Ray, you can understand how your application and its underlying services are performing to identify and troubleshoot the root cause of performance issues and errors.

TODO

[[top]](#python-test-samples)

## Cleanup

To delete the sample application that you created, use the AWS CLI. Assuming you used your project name for the stack name, you can run the following:

```bash
aws cloudformation delete-stack --stack-name python-test-samples
```

## Additional Resources

See the [AWS SAM developer guide](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/what-is-sam.html) for an introduction to SAM specification, the SAM CLI, and serverless application concepts.

Next, you can use AWS Serverless Application Repository to deploy ready to use Apps that go beyond hello world samples and learn how authors developed their applications: [AWS Serverless Application Repository main page](https://aws.amazon.com/serverless/serverlessrepo/)
