﻿#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          © West Wind Technologies, 2016
 *          http://www.west-wind.com/
 * 
 * Created: 04/28/2016
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************  
*/
#endregion

using System.Text.RegularExpressions;
using Markdig;
using Westwind.Utilities;

namespace MarkdownMonster
{

    /// <summary>
    /// Wrapper around the CommonMark.NET parser that provides a cached
    /// instance of the Markdown parser. Hooks up custom processing.
    /// </summary>
    public class  MarkdownParserMarkdig : MarkdownParserBase
    {
        public static MarkdownPipeline Pipeline;
        

        public MarkdownParserMarkdig(bool usePragmaLines = false, bool force = false)
        {
            if (force || Pipeline == null)
            {
                var builder = new MarkdownPipelineBuilder()
                    .UseYamlFrontMatter()
                    .UseEmphasisExtras()
                    .UseAutoLinks()
                    .UsePipeTables()
                    .UseGridTables()
                    .UseFooters()
                    .UseFootnotes()
                    .UseCitations()
                    .UseFigures();
                    
                if (usePragmaLines)
                    builder = builder
                        .UsePragmaLines();

                Pipeline = builder.Build();         
            }
        }

        /// <summary>
        /// Parses the actual markdown down to html
        /// </summary>
        /// <param name="markdown"></param>
        /// <returns></returns>
        public override string Parse(string markdown, bool renderLinksExternal = false)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            var html = Markdown.ToHtml(markdown, Pipeline);
            
            html = ParseFontAwesomeIcons(html);

            if (renderLinksExternal)
                html = ParseExternalLinks(html);

            if (!mmApp.Configuration.AllowRenderScriptTags)
                html = ParseScript(html);  
                      
            return html;
        }
    }

}
