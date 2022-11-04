# HighscoreApi Library Reference

The WebFramework Library is an Example implementation using the [`WebFramework Library`] to implement a simple Highscore API.

## Supported Endpoints

- `GET /get?name=<name>&apiKey=<apiKey>`
- `GET /list?apiKey=<apiKey>`
- `POST /set?apiKey=<apiKey>`
	Body: 
	```json
		{
			"name": "Name",
			"score": 123
		}
	```

## Starting the Highscore API
```js
const key = "MY_API_KEY";
const prefix = "http://localhost:3000/";

const Package = Runtime.Import("Package");
const HighscoreApi = Package.Import("HighscoreApi");

const app = new HighscoreApi(prefix, key);
app.Start();
```

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Common Libraries](./Readme.md)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)