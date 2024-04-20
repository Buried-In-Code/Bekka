global using log4net.Appender;
global using log4net.Config;
global using log4net.Layout;

global using NUnit.Framework;

namespace Bekka.Test;

[SetUpFixture]
public class Setup
{
    [OneTimeSetUp]
    public void BeforeAnyTests()
    {
        var patternLayout = new PatternLayout { ConversionPattern = "[%date{yyyy-MM-dd HH:mm:ss}] [%-5level] {%logger{2}} | %message%newline" };
        patternLayout.ActivateOptions();
        var consoleAppender = new ConsoleAppender { Layout = patternLayout };
        consoleAppender.ActivateOptions();
        BasicConfigurator.Configure(appender: consoleAppender);
    }
}