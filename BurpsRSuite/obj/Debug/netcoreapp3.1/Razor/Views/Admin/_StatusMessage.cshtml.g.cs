#pragma checksum "C:\Users\nignog\source\repos\BurpsRSuite\BurpsRSuite\Views\Admin\_StatusMessage.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5f66948ac7e602e4b0b96488e1c61d31d81a58a8"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Admin__StatusMessage), @"mvc.1.0.view", @"/Views/Admin/_StatusMessage.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\nignog\source\repos\BurpsRSuite\BurpsRSuite\Views\_ViewImports.cshtml"
using BurpsRSuite;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\nignog\source\repos\BurpsRSuite\BurpsRSuite\Views\_ViewImports.cshtml"
using BurpsRSuite.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\nignog\source\repos\BurpsRSuite\BurpsRSuite\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"5f66948ac7e602e4b0b96488e1c61d31d81a58a8", @"/Views/Admin/_StatusMessage.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"61022ae8cd04884714fcb953f755012c4e9f89c1", @"/Views/_ViewImports.cshtml")]
    public class Views_Admin__StatusMessage : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<string>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(" \r\n");
#nullable restore
#line 3 "C:\Users\nignog\source\repos\BurpsRSuite\BurpsRSuite\Views\Admin\_StatusMessage.cshtml"
 if (!String.IsNullOrEmpty(Model))
{
    var statusMessageClass = Model.StartsWith("Error") ? "danger" : "success";

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div");
            BeginWriteAttribute("class", " class=\"", 145, "\"", 212, 6);
            WriteAttributeValue("", 153, "alert", 153, 5, true);
            WriteAttributeValue(" ", 158, "alert-", 159, 7, true);
#nullable restore
#line 6 "C:\Users\nignog\source\repos\BurpsRSuite\BurpsRSuite\Views\Admin\_StatusMessage.cshtml"
WriteAttributeValue("", 165, statusMessageClass, 165, 19, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue(" ", 184, "alert-dismissible", 185, 18, true);
            WriteAttributeValue(" ", 202, "fade", 203, 5, true);
            WriteAttributeValue(" ", 207, "show", 208, 5, true);
            EndWriteAttribute();
            WriteLiteral(" role=\"alert\">\r\n        <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>\r\n        ");
#nullable restore
#line 8 "C:\Users\nignog\source\repos\BurpsRSuite\BurpsRSuite\Views\Admin\_StatusMessage.cshtml"
   Write(Model);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </div>\r\n");
#nullable restore
#line 10 "C:\Users\nignog\source\repos\BurpsRSuite\BurpsRSuite\Views\Admin\_StatusMessage.cshtml"
}

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<string> Html { get; private set; }
    }
}
#pragma warning restore 1591