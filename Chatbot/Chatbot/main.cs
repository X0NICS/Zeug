string endpoint = "http://127.0.0.1:11434";
HttpClient client = new HttpClient();

client.BaseAddress = new Uri(endpoint);

HttpResponseMessage availabilityResponse = await client.GetAsync("/api/tags");

string stringResponse = await availabilityResponse.Content.ReadAsStringAsync();

Console.WriteLine(stringResponse);