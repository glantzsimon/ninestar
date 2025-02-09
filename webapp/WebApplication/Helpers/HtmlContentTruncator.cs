using HtmlAgilityPack;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace K9.WebApplication.Helpers
{
    public static class HtmlContentTruncator
    {
        public static MvcHtmlString TruncateAndKeepFirstTwoParagraphs(string htmlFragment)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml($"<root>{htmlFragment}</root>"); // Wrap in <root> for safe parsing

            var root = doc.DocumentNode.SelectSingleNode("//root");
            if (root == null) return new MvcHtmlString(htmlFragment); // Safety check

            var allParagraphs = root.SelectNodes("//p"); // Find all paragraphs, even nested
            if (allParagraphs == null || allParagraphs.Count == 0) return new MvcHtmlString(htmlFragment);

            HtmlNode firstParagraph = allParagraphs[0]; // First paragraph is kept as-is
            HtmlNode secondParagraph = null;

            if (allParagraphs.Count > 1)
            {
                secondParagraph = allParagraphs[1];
                TruncateParagraphKeepingHTML(secondParagraph, 7); // Truncate second paragraph to 7 words
            }

            // Find and keep only parent containers of the first two paragraphs
            var preservedNodes = root.SelectNodes("//*").Where(node =>
                node == firstParagraph || node == secondParagraph || IsAncestorOf(firstParagraph, node) || IsAncestorOf(secondParagraph, node)
            ).ToList();

            // Remove everything except the preserved elements
            foreach (var node in root.SelectNodes("//*").ToList())
            {
                if (!preservedNodes.Contains(node))
                {
                    node.Remove();
                }
            }

            return new MvcHtmlString(string.Join("", root.ChildNodes.Select(n => n.OuterHtml))); // Return final HTML
        }

        private static void TruncateParagraphKeepingHTML(HtmlNode paragraph, int maxWords)
        {
            int wordsCount = 0;

            // Traverse all child nodes without modifying structure
            foreach (var node in paragraph.ChildNodes.ToList())
            {
                if (node.NodeType == HtmlNodeType.Text)
                {
                    var words = Regex.Split(node.InnerText.Trim(), @"\s+");
                    var remainingWords = words.Take(maxWords - wordsCount).ToArray();
                    wordsCount += remainingWords.Length;

                    if (wordsCount >= maxWords)
                    {
                        node.InnerHtml = string.Join(" ", remainingWords) + " ...";

                        // Remove all siblings (including inline elements) after truncation
                        while (node.NextSibling != null)
                        {
                            node.NextSibling.Remove();
                        }
                        return;
                    }
                }
                else if (node.NodeType == HtmlNodeType.Element)
                {
                    // If it's an inline element (e.g., <strong>, <em>), process it recursively
                    TruncateParagraphKeepingHTML(node, maxWords - wordsCount);
                    wordsCount += node.InnerText.Split(' ').Length;

                    if (wordsCount >= maxWords)
                    {
                        // Remove all following siblings after truncation
                        while (node.NextSibling != null)
                        {
                            node.NextSibling.Remove();
                        }
                        return;
                    }
                }
            }
        }

        private static bool IsAncestorOf(HtmlNode parent, HtmlNode child)
        {
            while (child != null)
            {
                if (child.ParentNode == parent) return true;
                child = child.ParentNode;
            }
            return false;
        }
    }
}