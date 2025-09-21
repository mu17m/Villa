using System;
using System.Collections.Generic;
using System.IO;
using NPOI;
using Microsoft.AspNetCore.Mvc;
using NPOI.XWPF.UserModel;

public class WordExportService
{
    public byte[] GenerateBookingDocument(string templatePath, Dictionary<string, string> replacements, List<List<string>> tableData)
    {
        using (FileStream stream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
        {
            XWPFDocument document = new XWPFDocument(stream);

            // Replace placeholders in paragraphs
            foreach (XWPFParagraph paragraph in document.Paragraphs)
            {
                foreach (var placeholder in replacements)
                {
                    if (paragraph.Text.Contains(placeholder.Key))
                    {
                        paragraph.ReplaceText(placeholder.Key, placeholder.Value);
                    }
                }
            }

            // Replace placeholders inside tables
            foreach (XWPFTable table in document.Tables)
            {
                foreach (XWPFTableRow row in table.Rows)
                {
                    foreach (XWPFTableCell cell in row.GetTableCells())
                    {
                        foreach (var placeholder in replacements)
                        {
                            if (cell.GetText().Contains(placeholder.Key))
                            {
                                cell.RemoveParagraph(0);
                                cell.SetText(placeholder.Value);
                            }
                        }
                    }
                }
            }

            // Find <ADDTABLEHERE> and replace it with a new table
            foreach (XWPFParagraph paragraph in document.Paragraphs)
            {
                if (paragraph.Text.Contains("<ADDTABLEHERE>"))
                {
                    int position = document.Paragraphs.IndexOf(paragraph);
                    document.RemoveBodyElement(position); // Remove placeholder

                    // Create a new table
                    XWPFTable newTable = document.CreateTable(tableData.Count, tableData[0].Count);

                    for (int i = 0; i < tableData.Count; i++)
                    {
                        for (int j = 0; j < tableData[i].Count; j++)
                        {
                            newTable.GetRow(i).GetCell(j).SetText(tableData[i][j]);
                        }
                    }
                    break;
                }
            }

            // Save document to memory
            using (MemoryStream memoryStream = new MemoryStream())
            {
                document.Write(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
