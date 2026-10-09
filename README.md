# EventApp
ASP.NET app to manage events
Run the app with `dotnet run --project EventApp .

## HTTP API

The current service is in-memory. ID is new at every app launch.

*Endpoint documentation prepared by Codex.*

The `{id}` route parameter must be a GUID. POST and PUT requests must send a JSON body
with the `Content-Type: application/json` header.

| Method | Route | Description | Successful response | Expected errors |
| --- | --- | --- | --- | --- |
| GET | `/events` | Returns all events. No ID is required. | `200 OK` with an array of events; `[]` if there are no events. | — |
| GET | `/events/{id}` | Returns the event with the specified ID. | `200 OK` with an event. | `400 Bad Request` for an invalid GUID; `404 Not Found` if the event does not exist. |
| POST | `/events` | Creates an event from the request body. | `201 Created` with no response body. | `400 Bad Request` for invalid request data; `415 Unsupported Media Type` for an unsupported body format. |
| PUT | `/events/{id}` | Updates the title, dates and description of an existing event. | `200 OK` with no response body. | `400 Bad Request` for an invalid GUID or request data; `404 Not Found` if the event does not exist; `415 Unsupported Media Type` for an unsupported body format. |
| DELETE | `/events/{id}` | Deletes the event with the specified ID. | `200 OK` with no response body. | `400 Bad Request` for an invalid GUID; `404 Not Found` if the event does not exist. |

POST and PUT use the same request body:

```json
{
  "title": "Conference",
  "startAt": "2026-10-09T12:00:00+03:00",
  "endAt": "2026-10-09T14:00:00+03:00",
  "description": "Event description"
}
```

The title must contain 2–100 characters and cannot be blank. `endAt` must be later than
`startAt`. GET responses include the fields `guid`, `title`, `startAt`, `endAt` and
`description`; use `guid` as the `{id}` parameter for subsequent requests.

