// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.AppServices.SourceGenerators.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.CSharp;

namespace CommunityToolkit.AppServices.SourceGenerators.Tests;

[TestClass]
public class Test_SourceGenerators_Diagnostics
{
    [TestMethod]
    public async Task Verify_InvalidAppServicesMemberType_GenericInstanceMethod()
    {
        string source = """
            using CommunityToolkit.AppServices;

            namespace MyApp
            {
                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public void {|APPSRVSPR0001:Foo|}<T>();
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidAppServicesMemberAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidAppServicesMemberType_Property()
    {
        string source = """
            using CommunityToolkit.AppServices;

            namespace MyApp
            {
                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public int {|APPSRVSPR0001:Bar|} { get; set; }
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidAppServicesMemberAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidAppServicesMethodReturnType_Void()
    {
        string source = """
            using CommunityToolkit.AppServices;

            namespace MyApp
            {
                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public void {|APPSRVSPR0002:Foo|}();
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidAppServicesMemberAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidAppServicesMethodReturnType_Object()
    {
        string source = """
            using CommunityToolkit.AppServices;

            namespace MyApp
            {
                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public object {|APPSRVSPR0002:Foo|}();
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidAppServicesMemberAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidAppServicesMethodReturnType_Dynamic()
    {
        string source = """
            using CommunityToolkit.AppServices;

            namespace MyApp
            {
                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public dynamic {|APPSRVSPR0002:Foo|}();
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidAppServicesMemberAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidAppServicesMethodReturnType_MDArray()
    {
        string source = """
            using CommunityToolkit.AppServices;

            namespace MyApp
            {
                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public int[,] {|APPSRVSPR0002:Foo|}();
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidAppServicesMemberAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidAppServicesMethodReturnType_CustomTypeWithNoSerializer()
    {
        string source = """
            using CommunityToolkit.AppServices;

            namespace MyApp
            {
                public class MyClass
                {
                }

                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public MyClass {|APPSRVSPR0002:Foo|}();
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidAppServicesMemberAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidAppServicesMethodParameterType_Object()
    {
        string source = """
            using System.Threading.Tasks;
            using CommunityToolkit.AppServices;

            namespace MyApp
            {
                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public Task FooAsync(object {|APPSRVSPR0003:input|});
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidAppServicesMemberAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidAppServicesMethodParameterType_MDArray()
    {
        string source = """
            using System.Threading.Tasks;
            using CommunityToolkit.AppServices;

            namespace MyApp
            {
                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public Task FooAsync(int[,] {|APPSRVSPR0003:input|});
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidAppServicesMemberAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidAppServicesMethodParameterType_CustomTypeWithNoSerializer()
    {
        string source = """
            using System.Threading.Tasks;
            using CommunityToolkit.AppServices;

            namespace MyApp
            {
                public class MyClass
                {
                }

                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public Task FooAsync(MyClass {|APPSRVSPR0003:input|});
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidAppServicesMemberAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidRepeatedAppServicesMethodIProgressParameter()
    {
        string source = """
            using System;
            using System.Threading.Tasks;
            using CommunityToolkit.AppServices;

            namespace MyApp
            {
                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public Task FooAsync(IProgress<int> progress1, IProgress<string> {|APPSRVSPR0004:progress2|});
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidAppServicesMemberAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidRepeatedAppServicesMethodCancellationTokenParameter()
    {
        string source = """
            using System.Threading;
            using System.Threading.Tasks;
            using CommunityToolkit.AppServices;

            namespace MyApp
            {
                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public Task FooAsync(CancellationToken token1, CancellationToken {|APPSRVSPR0005:token2|});
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidAppServicesMemberAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidValueSetSerializerType_NoPublicParameterlessConstructor()
    {
        string source = """
            using System.Threading.Tasks;
            using CommunityToolkit.AppServices;
            using Windows.Foundation.Collections;

            namespace MyApp
            {
                public class MyClass
                {
                    public string Text { get; set; }
                }

                public class MyClassSerializer : IValueSetSerializer<MyClass>
                {
                    public MyClassSerializer(object dummy)
                    {
                    }

                    MyClass? IValueSetSerializer<MyClass>.Deserialize(ValueSet? valueSet)
                    {
                        return null;
                    }

                    ValueSet? IValueSetSerializer<MyClass>.Serialize(MyClass? value)
                    {
                        return null;
                    }
                }

                [AppService("MyAppService")]
                public interface IMyAppService
                {
                    public Task FooAsync([{|APPSRVSPR0006:ValueSetSerializer(typeof(MyClassSerializer))|}] MyClass input);
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidValueSetSerializerUseAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidValueSetSerializerLocation_ReturnType()
    {
        string source = """
            using System.Threading.Tasks;
            using CommunityToolkit.AppServices;
            using Windows.Foundation.Collections;

            namespace MyApp
            {
                public class MyClass
                {
                    public string Text { get; set; }
                }

                public class MyClassSerializer : IValueSetSerializer<MyClass>
                {
                    public MyClassSerializer(object dummy)
                    {
                    }

                    MyClass? IValueSetSerializer<MyClass>.Deserialize(ValueSet? valueSet)
                    {
                        return null;
                    }

                    ValueSet? IValueSetSerializer<MyClass>.Serialize(MyClass? value)
                    {
                        return null;
                    }
                }

                public class TestClass
                {
                    [return: {|APPSRVSPR0007:ValueSetSerializer(typeof(MyClassSerializer))|}]
                    public MyClass Foo()
                    {
                        return null;
                    }
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidValueSetSerializerUseAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidValueSetSerializerLocation_MethodParameter()
    {
        string source = """
            using System.Threading.Tasks;
            using CommunityToolkit.AppServices;
            using Windows.Foundation.Collections;

            namespace MyApp
            {
                public class MyClass
                {
                    public string Text { get; set; }
                }

                public class MyClassSerializer : IValueSetSerializer<MyClass>
                {
                    public MyClassSerializer(object dummy)
                    {
                    }

                    MyClass? IValueSetSerializer<MyClass>.Deserialize(ValueSet? valueSet)
                    {
                        return null;
                    }

                    ValueSet? IValueSetSerializer<MyClass>.Serialize(MyClass? value)
                    {
                        return null;
                    }
                }

                public class TestClass
                {
                    public void Foo([{|APPSRVSPR0007:ValueSetSerializer(typeof(MyClassSerializer))|}] MyClass input)
                    {
                    }
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidValueSetSerializerUseAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task Verify_InvalidValueSetSerializerLocation_LocalFunctionReturnType()
    {
        string source = """
            using System.Threading.Tasks;
            using CommunityToolkit.AppServices;
            using Windows.Foundation.Collections;

            namespace MyApp
            {
                public class MyClass
                {
                    public string Text { get; set; }
                }

                public class MyClassSerializer : IValueSetSerializer<MyClass>
                {
                    public MyClassSerializer(object dummy)
                    {
                    }

                    MyClass? IValueSetSerializer<MyClass>.Deserialize(ValueSet? valueSet)
                    {
                        return null;
                    }

                    ValueSet? IValueSetSerializer<MyClass>.Serialize(MyClass? value)
                    {
                        return null;
                    }
                }

                public class TestClass
                {
                    public void Foo()
                    {
                        [return: {|APPSRVSPR0007:ValueSetSerializer(typeof(MyClassSerializer))|}]
                        MyClass Foo()
                        {
                            return null;
                        }
                    }
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidValueSetSerializerUseAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp9);
    }

    [TestMethod]
    public async Task Verify_InvalidValueSetSerializerLocation_LocalFunctionParameter()
    {
        string source = """
            using System.Threading.Tasks;
            using CommunityToolkit.AppServices;
            using Windows.Foundation.Collections;

            namespace MyApp
            {
                public class MyClass
                {
                    public string Text { get; set; }
                }

                public class MyClassSerializer : IValueSetSerializer<MyClass>
                {
                    public MyClassSerializer(object dummy)
                    {
                    }

                    MyClass? IValueSetSerializer<MyClass>.Deserialize(ValueSet? valueSet)
                    {
                        return null;
                    }

                    ValueSet? IValueSetSerializer<MyClass>.Serialize(MyClass? value)
                    {
                        return null;
                    }
                }

                public class TestClass
                {
                    public void Foo()
                    {
                        void Foo([{|APPSRVSPR0007:ValueSetSerializer(typeof(MyClassSerializer))|}] MyClass input)
                        {
                        }
                    }
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidValueSetSerializerUseAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp9);
    }

    [TestMethod]
    public async Task Verify_InvalidValueSetSerializerLocation_LambdaReturnType()
    {
        string source = """
            using System;
            using System.Threading.Tasks;
            using CommunityToolkit.AppServices;
            using Windows.Foundation.Collections;

            namespace MyApp
            {
                public class MyClass
                {
                    public string Text { get; set; }
                }

                public class MyClassSerializer : IValueSetSerializer<MyClass>
                {
                    public MyClassSerializer(object dummy)
                    {
                    }

                    MyClass? IValueSetSerializer<MyClass>.Deserialize(ValueSet? valueSet)
                    {
                        return null;
                    }

                    ValueSet? IValueSetSerializer<MyClass>.Serialize(MyClass? value)
                    {
                        return null;
                    }
                }

                public class TestClass
                {
                    public void Foo()
                    {
                        Func<MyClass> action = [return: {|APPSRVSPR0007:ValueSetSerializer(typeof(MyClassSerializer))|}] () => new MyClass();
                    }
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidValueSetSerializerUseAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp10);
    }

    [TestMethod]
    public async Task Verify_InvalidValueSetSerializerLocation_LambdaParameter()
    {
        string source = """
            using System;
            using System.Threading.Tasks;
            using CommunityToolkit.AppServices;
            using Windows.Foundation.Collections;

            namespace MyApp
            {
                public class MyClass
                {
                    public string Text { get; set; }
                }

                public class MyClassSerializer : IValueSetSerializer<MyClass>
                {
                    public MyClassSerializer(object dummy)
                    {
                    }

                    MyClass? IValueSetSerializer<MyClass>.Deserialize(ValueSet? valueSet)
                    {
                        return null;
                    }

                    ValueSet? IValueSetSerializer<MyClass>.Serialize(MyClass? value)
                    {
                        return null;
                    }
                }

                public class TestClass
                {
                    public void Foo()
                    {
                        Action<MyClass> action = ([{|APPSRVSPR0007:ValueSetSerializer(typeof(MyClassSerializer))|}] MyClass input) => { };
                    }
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<InvalidValueSetSerializerUseAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp10);
    }
}
