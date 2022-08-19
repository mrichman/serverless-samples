.PHONY: build test deploy http

build:
	sam build

test: build
	cd hello-world && go test -v
	sam local invoke -e ./events/event.json

deploy: build
	sam deploy

http:
	echo '{"hello": "world"}' | http POST https://trwsk9qn2g.execute-api.us-east-1.amazonaws.com/Prod/hello/
