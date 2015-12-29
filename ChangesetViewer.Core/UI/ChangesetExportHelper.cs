using System.Globalization;
using ChangesetViewer.Core.Model;
using ChangesetViewer.Core.TFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using PluginCore.Extensions;
using System.Collections.ObjectModel;

namespace ChangesetViewer.Core.UI
{
    public class ChangesetExportHelper
    {
        private IList<ChangesetViewModel> _changesetCollection;
        private ObservableCollection<ChangesetViewModel> _observChangesets;
        private Action _action1;
        private Action _action2;

        private readonly Excel.Application _xApp;
        private readonly Excel.Workbook _xlWorkBook;
        private readonly Excel.Worksheet _xlWorkSheet;
        private Excel.Range _xRange;

        private const int HeaderRow = 4;

        public ChangesetExportHelper()
        {
            _xApp = new Excel.Application();
            _xlWorkBook = _xApp.Workbooks.Add();
            _xlWorkSheet = (Excel.Worksheet)_xlWorkBook.Worksheets.Item[1];
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

            _xRange = _xlWorkSheet.Range["B:B", Type.Missing];
            _xRange.Columns.ColumnWidth = 10;
            _xRange.Columns.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

            _xRange = _xlWorkSheet.Range["C:C", Type.Missing];
            _xRange.Columns.ColumnWidth = 15;
            _xRange.Columns.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

            _xRange = _xlWorkSheet.Range["D:D", Type.Missing];
            _xRange.Columns.ColumnWidth = 25;

            _xRange = _xlWorkSheet.Range["E:E", Type.Missing];
            _xRange.Columns.ColumnWidth = 40;

            _xRange = _xlWorkSheet.Range["F:F", Type.Missing];
            _xRange.Columns.ColumnWidth = 100;
            _xRange.Columns.WrapText = true;

            _xRange = _xlWorkSheet.Range["G:G", Type.Missing];
            _xRange.Columns.ColumnWidth = 15;
            _xRange.Columns.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

            var includeCol = FuncExtensions.Create((string col) => string.Format("{0}{1}", col, HeaderRow));

            var headerColumns = new[] { includeCol("B"), includeCol("C"), includeCol("D"), includeCol("E"), includeCol("F"), includeCol("G") };

            var applyFormatToHeader = ActionExtensions.Create((string col) =>
            {
                _xRange = _xlWorkSheet.Range[col, Type.Missing];
                if (_xRange == null) return;
                _xRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                _xRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
            });

            headerColumns.Iter(applyFormatToHeader);

            _xlWorkSheet.Cells[HeaderRow, 2] = "SL No";
            _xlWorkSheet.Cells[HeaderRow, 3] = "Id";
            _xlWorkSheet.Cells[HeaderRow, 4] = "Committer";
            _xlWorkSheet.Cells[HeaderRow, 5] = "Check-in Date";
            _xlWorkSheet.Cells[HeaderRow, 6] = "Comment";
            _xlWorkSheet.Cells[HeaderRow, 7] = "Work Item Ids";

        }


        public void ExportToExcel(ObservableCollection<ChangesetViewModel> changesets, Action enableUiControlsLevel1, Action enableUiControlsLevel2)
        {
            _observChangesets = changesets;
            _action1 = enableUiControlsLevel1;
            _action2 = enableUiControlsLevel2;
            _changesetCollection = new List<ChangesetViewModel>();

            Task.Factory.StartNew(ProcessExportToExcel);
        }

        private void ProcessExportToExcel()
        {
            _observChangesets.ToArray()
                .Iter(c => _changesetCollection.Add(new ChangesetViewModel
                {
                    ChangesetId = c.ChangesetId,
                    Comment = c.Comment,
                    CommitterDisplayName = c.CommitterDisplayName,
                    CreationDate = c.CreationDate,
                    WorkItemIds = c.WorkItemIds
                }
                    ));

            _action1();

            if (_changesetCollection.Count == 0)
            {
                _action2();
                return;
            }

            exportToExcel();
        }

        private void exportToExcel()
        {
            try
            {
                var rowsCounter = HeaderRow + 1;
                var counter = 1;
                InitializeHeaders();

                _changesetCollection.Iter(changeset =>
                {
                    _xlWorkSheet.Cells[rowsCounter, 2] = counter.ToString(CultureInfo.InvariantCulture);
                    _xlWorkSheet.Cells[rowsCounter, 3] = changeset.ChangesetId;
                    _xlWorkSheet.Cells[rowsCounter, 4] = changeset.CommitterDisplayName;
                    _xlWorkSheet.Cells[rowsCounter, 5] = changeset.CreationDate.ToLongDateString() + " " + changeset.CreationDate.ToLongTimeString();
                    _xlWorkSheet.Cells[rowsCounter, 6] = changeset.Comment;
                    _xlWorkSheet.Cells[rowsCounter, 7] = string.Join(", ", changeset.WorkItemIds.Split(",".ToCharArray()).Select(w => w.Trim()));
                    if (changeset.WorkItemTitles.HasValue())
                        ((Excel.Range)_xlWorkSheet.Cells[rowsCounter, 7]).AddComment(string.Join(", ", changeset.WorkItemTitles.Split(",".ToCharArray()).Select(w => w.Trim())));

                    counter++;
                    rowsCounter++;
                });

                var fileName = _xApp.GetSaveAsFilename();

                if (!string.IsNullOrEmpty(fileName))
                {
                    if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
                        fileName = string.Format("{0}.{1}", ((string)fileName).RemoveLastIfLast("."), "xlsx");
                    _xlWorkBook.SaveAs(fileName);
                }
            }
            catch
            { }
            finally
            {
                _xlWorkBook.Close(false);
                _xApp.Quit();

                ReleaseObject(_xlWorkSheet);
                ReleaseObject(_xlWorkBook);
                ReleaseObject(_xApp);

                _action2();
            }
        }

        private static void ReleaseObject(object obj)
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
