using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace MyNamespace
{

    class Program
    {
        static async Task Main(string[] args)
        {
            RootCommand? idbox = new("idbox");
            Command? box = new("box");
            Command? create = new("create");
            Command? open = new("open");
            Command? ids = new("ids");
            Command? list = new("list");
            idbox.Add(box);
            box.Add(create);
            box.Add(open);
            idbox.Add(ids);
            ids.Add(list);

            Argument argument = new("fileName");
            create.AddArgument(argument);
            create.Handler = CommandHandler.Create<string>(CreateHandler);

            open.AddArgument(argument);
            open.Handler = CommandHandler.Create<string>(OpenHandler);

            Option<bool> summaryOption = new("--summary", () => true);
            Option<bool> detailOption = new("--detail", () => false);
            list.AddOption(summaryOption);
            list.AddOption(detailOption);
            list.Handler = CommandHandler.Create(ListHandler);


            await idbox.InvokeAsync(args);

            void CreateHandler(string fileName)
            {
                FileInfo? fileInfo = new(Path.Combine(Path.GetTempPath(), fileName));
                using StreamWriter? writer = new(fileInfo.FullName);
                writer.WriteLine("item1");
                writer.WriteLine("item2");

                Console.WriteLine($"IdBox created from location {Path.Combine(Path.GetTempPath(), fileName)}");
            }

            async Task OpenHandler(string fileName)
            {
                string text = await File.ReadAllTextAsync(Path.Combine(Path.GetTempPath(), fileName));
                await File.WriteAllTextAsync(Path.Combine(Path.GetTempPath(), "open"), text);

                Console.WriteLine($"IdBox opened from location {Path.Combine(Path.GetTempPath(), fileName)}");
            }

            async Task ListHandler(bool detail, bool summary)
            {
                string text = await File.ReadAllTextAsync(Path.Combine(Path.GetTempPath(), "open"));

                if (detail)
                    Console.WriteLine("Detail!");
                else if (summary)
                    Console.WriteLine("Summary!");

                Console.WriteLine(text);
            }
        }
    }
}
