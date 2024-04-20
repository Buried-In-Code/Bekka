# Bekka

![.NET Version](https://img.shields.io/badge/.Net-8-green?style=flat-square&logo=csharp)
![Status](https://img.shields.io/badge/Status-Beta-yellowgreen?style=flat-square)

[![Github - Version](https://img.shields.io/github/v/tag/Buried-In-Code/Bekka?logo=Github&label=Version&style=flat-square)](https://github.com/Buried-In-Code/Bekka/tags)
[![Github - License](https://img.shields.io/github/license/Buried-In-Code/Bekka?logo=Github&label=License&style=flat-square)](https://opensource.org/licenses/MIT)
[![Github - Contributors](https://img.shields.io/github/contributors/Buried-In-Code/Bekka?logo=Github&label=Contributors&style=flat-square)](https://github.com/Buried-In-Code/Bekka/graphs/contributors)

A C#/.Net wrapper for the [metron.cloud](https://metron.cloud) API.

## Installation

```sh
dotnet add package Bekka
```

## Example Usage

```csharp
using Bekka;

namespace Program
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var (username, password) = LoadConfig(); // Load your credentials from config file or env variables

            var session = new Metron(username: username, password: password);

            // Get all Marvel comics for the week of 2023-Apr-15
            var weeklyComics = await session.ListIssues(parameters: new Dictionary<string, string>
            {
                { "store_date_range_after", "2023-04-14" },
                { "store_date_range_before", "2023-04-22" },
                { "publisher_name", "marvel" }
            });
            // Print the results
            foreach (var comic in weeklyComics)
            {
                Console.WriteLine($"{comic.Id} {comic.IssueName}");
            }

            // Retrieve the details for an individual issue
            var spiderMan68 = await session.GetIssue(id: 31660);
            // Print the issue Description
            Console.WriteLine(spiderMan68.Description);
        }
    }
}
```
