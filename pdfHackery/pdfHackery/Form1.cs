using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace pdfHackery
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            SplitButton.Click += (sender, args) => SplitDocument();
        }

        public void SplitDocument()
        {
            var documentPath = @"page1.pdf";
            var outputPath = @"pagesplit.pdf";

            var document = new Document();
            var reader = new PdfReader(documentPath);

            using (var stream = new MemoryStream())
            {
                var writer = PdfWriter.GetInstance(document, stream);
                document.Open();
                document.AddDocListener(writer);
                var pagesToExtract = new List<int> { 2, 3 };

                for (var page = 1; page < reader.NumberOfPages; page++)
                {
                    if (pagesToExtract.Contains(page))
                    {
                        document.SetPageSize(reader.GetPageSize(page));
                        document.NewPage();

                        var contentByte = writer.DirectContent;
                        var pageImport = writer.GetImportedPage(reader, page);

                        var rotation = reader.GetPageRotation(page);

                        if (rotation == 90 || rotation == 270)
                        {
                            contentByte.AddTemplate(pageImport, 0, -1.0F, 1.0F, 0, 0, reader.GetPageSizeWithRotation(page).Height);
                        }
                        else
                        {
                            contentByte.AddTemplate(pageImport, 1.0F, 0, 0, 1.0F, 0, 0);
                        }                               
                    }
                }

                reader.Close();
                document.Close();
                File.WriteAllBytes(outputPath, stream.ToArray());
            }
        }
    }
}
