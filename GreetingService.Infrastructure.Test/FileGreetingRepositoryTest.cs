using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;
using GreetingService.API.Core;

namespace GreetingService.Infrastructure.Test
{
    namespace GreetingService.Infrastructure.Test
    {
        public class FileGreetingRepositoryTest
        {
            public FileGreetingRepository _repository { get; set; }

            private readonly string _filePath;
            private readonly List<Greeting> _testData;

            public FileGreetingRepositoryTest()
            {
                _filePath = $"greeting_unit_test_{DateTime.Now:yyyyMMddHHmmss}.json";
                _repository = new FileGreetingRepository(_filePath);

                _testData = new List<Greeting>
            {
                new Greeting
                {
                    From = "from1",
                    To = "to1",
                    Message = "message1",
                },
                new Greeting
                {
                    From = "from2",
                    To = "to2",
                    Message = "message2",
                },
                new Greeting
                {
                    From = "from3",
                    To = "to3",
                    Message = "message3",
                },
                new Greeting
                {
                    From = "from4",
                    To = "to4",
                    Message = "message4",
                },
            };

                File.WriteAllText(_filePath, JsonSerializer.Serialize(_testData, new JsonSerializerOptions { WriteIndented = true }));
            }

            [Fact]
            public void get_should_return_empty_collection()
            {
                var greetings = _repository.Get();
                Assert.NotNull(greetings);
                Assert.NotEmpty(greetings);
                Assert.Equal(_testData.Count(), greetings.Count());
            }

            [Fact]
            public void get_should_return_correct_greeting()
            {
                var expectedGreeting1 = _testData[0];
                var actualGreeting1 = _repository.Get(expectedGreeting1.Id);
                Assert.NotNull(actualGreeting1);
                Assert.Equal(expectedGreeting1.Id, actualGreeting1.Id);

                var expectedGreeting2 = _testData[1];
                var actualGreeting2 = _repository.Get(expectedGreeting2.Id);
                Assert.NotNull(actualGreeting2);
                Assert.Equal(expectedGreeting2.Id, actualGreeting2.Id);
            }

            [Fact]
            public void post_should_persist_to_file()
            {
                var greetingsBeforeCreate = _repository.Get();

                var newGreeting = new Greeting
                {
                    From = "post_test",
                    To = "post_test",
                    Message = "post_test",
                };

                _repository.Create(newGreeting);

                var greetingsAfterCreate = _repository.Get();

                Assert.Equal(greetingsBeforeCreate.Count() + 1, greetingsAfterCreate.Count());
            }

            [Fact]
            public void update_should_persist_to_file()
            {
                var greetings = _repository.Get();

                var firstGreeting = greetings.First();
                var firstGreetingMessage = firstGreeting.Message;

                var testMessage = "new updated message";
                firstGreeting.Message = testMessage;

                _repository.Update(firstGreeting);

                var firstGreetingAfterUpdate = _repository.Get(firstGreeting.Id);
                Assert.Equal(testMessage, firstGreetingAfterUpdate.Message);
            }
        }
    }
}