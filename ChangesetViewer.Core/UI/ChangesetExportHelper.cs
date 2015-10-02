using ChangesetViewer.Core.TFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using PluginCore.Extensions;
using System.Collections.ObjectModel;

namespace ChangesetViewer.Core.UI
{
    public class ChangesetExportHelper
    {
        private ITfsServer _tfsServer;
        private ITfsChangsets _changesets;
        private IList<ChangesetViewModel> _changesetCollection;
        private ObservableCollection<ChangesetViewModel> _observChangesets;
        private Action action1;
        private Action action2;

        private Excel.Application xApp;
        private Excel.Workbook xlWorkBook;
        private Excel.Worksheet xlWorkSheet;
        private Excel.Range xRange;

        private const int _HeaderRow = 4;


        public ChangesetExportHelper(ITfsServer tfsServer)
        {
            _tfsServer = tfsServer;
            _changesets = new TfsChangesets(_tfsServer);

            xApp = new Excel.Application();
            xlWorkBook = xApp.Workbooks.Add();
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
        }

        private void InitializeHeaders()
        {
            //Reference:
            //  B : SL No
            //  C : Changeset Id
            //  D : Commiter
            //  E : Check in Date
            //  F : Comment
            //  G : WorkItem Ids

            xRange = xlWorkSheet.get_Range("B:B", System.Type.Missing);
            xRange.Columns.ColumnWidth = 10;
            xRange.Columns.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

            xRange = xlWorkSheet.get_Range("C:C", System.Type.Missing);
            xRange.Columns.ColumnWidth = 15;
            xRange.Columns.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

            xRange = xlWorkSheet.get_Range("D:D", System.Type.Missing);
            xRange.Columns.ColumnWidth = 25;

            xRange = xlWorkSheet.get_Range("E:E", System.Type.Missing);
            xRange.Columns.ColumnWidth = 40;

            xRange = xlWorkSheet.get_Range("F:F", System.Type.Missing);
            xRange.Columns.ColumnWidth = 100;
            xRange.Columns.WrapText = true;

            xRange = xlWorkSheet.get_Range("G:G", System.Type.Missing);
            xRange.Columns.ColumnWidth = 15;
            xRange.Columns.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

            var includeCol = FuncExtensions.Create((string col) =>
            {
                return string.Format("{0}{1}", col, _HeaderRow.ToString());
            });

            var headerColumns = new[] { includeCol("B"), includeCol("C"), includeCol("D"), includeCol("E"), includeCol("F"), includeCol("G") };

            var applyFormatToHeader = ActionExtensions.Create((string col) =>
            {
                xRange = xlWorkSheet.get_Range(col, System.Type.Missing);
                xRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                xRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
            });

            headerColumns.Iter(d => applyFormatToHeader(d));

            xlWorkSheet.Cells[_HeaderRow, 2] = "SL No";
            xlWorkSheet.Cells[_HeaderRow, 3] = "Id";
            xlWorkSheet.Cells[_HeaderRow, 4] = "Committer";
            xlWorkSheet.Cells[_HeaderRow, 5] = "Check-in Date";
            xlWorkSheet.Cells[_HeaderRow, 6] = "Comment";
            xlWorkSheet.Cells[_HeaderRow, 7] = "Work Item Ids";

        }


        public void ExportToExcel(ObservableCollection<ChangesetViewModel> changesets, Action enableUiControlsLevel1, Action enableUiControlsLevel2)
        {
            _observChangesets = changesets;
            action1 = enableUiControlsLevel1;
            action2 = enableUiControlsLevel2;
            _changesetCollection = new List<ChangesetViewModel>();

            Task.Factory.StartNew(() => ProcessExportToExcel());
        }

        private void ProcessExportToExcel()
        {
            _observChangesets.ToArray<ChangesetViewModel>().Iter(c =>
            {
                _changesetCollection.Add(new ChangesetViewModel
                {
                    ChangesetId = c.ChangesetId,
                    Comment = c.Comment,
                    CommitterDisplayName = c.CommitterDisplayName,
                    CreationDate = c.CreationDate,
                    WorkItemIds = c.WorkItemIds
                });
            });

            action1();

            if (_changesetCollection.Count == 0)
            {
                action2();
                return;
            }

            exportToExcel();
        }

        private void exportToExcel()
        {
            try
            {
                int rowsCounter = _HeaderRow + 1;
                int counter = 1;
                InitializeHeaders();

                _changesetCollection.Iter(changeset =>
                {
                    xlWorkSheet.Cells[rowsCounter, 2] = counter.ToString();
                    xlWorkSheet.Cells[rowsCounter, 3] = changeset.ChangesetId;
                    xlWorkSheet.Cells[rowsCounter, 4] = changeset.CommitterDisplayName;
                    xlWorkSheet.Cells[rowsCounter, 5] = changeset.CreationDate.ToLongDateString() + " " + changeset.CreationDate.ToLongTimeString();
                    xlWorkSheet.Cells[rowsCounter, 6] = changeset.Comment;
                    xlWorkSheet.Cells[rowsCounter, 7] = string.Join(", ", changeset.WorkItemIds.Split(",".ToCharArray()).Select(w => w.Trim()));
                    if (changeset.WorkItemTitles.HasValue())
                        ((Excel.Range)xlWorkSheet.Cells[rowsCounter, 7]).AddComment(string.Join(", ", changeset.WorkItemTitles.Split(",".ToCharArray()).Select(w => w.Trim())));

                    counter++;
                    rowsCounter++;
                });

                var fileName = xApp.GetSaveAsFilename();

                if (!string.IsNullOrEmpty(fileName))
                {
                    if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
                        fileName = string.Format("{0}.{1}", ((string)fileName).RemoveLastIfLast("."), "xlsx");
                    xlWorkBook.SaveAs(fileName);
                }
            }
            catch { }
            finally
            {
                xlWorkBook.Close(false);
                xApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xApp);

                action2();
            }
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;

            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
