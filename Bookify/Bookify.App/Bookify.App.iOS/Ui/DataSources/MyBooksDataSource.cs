using System;
using Bookify.App.Core.ViewModels;
using Bookify.App.iOS.Ui.Controllers;
using Bookify.App.iOS.Ui.Views;
using Foundation;
using UIKit;

namespace Bookify.App.iOS.Ui.DataSources
{
    public class MyBooksDataSource : UITableViewSource
    {
        private readonly MyBooksViewController parent;

        private readonly MyBooksViewModel viewModel;

        public MyBooksDataSource(MyBooksViewController parent, MyBooksViewModel viewModel)
        {
            this.parent = parent;
            this.viewModel = viewModel;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row >= this.viewModel.Books.Count - 1)
            {
                this.viewModel.Books.LoadMore();
            }
            return BookTableCell.CreateCell(tableView, indexPath, this.viewModel.Books[indexPath.Row]);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return section == 0 ? this.viewModel.Books.Count : 0;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            this.parent.CellTapped(this.viewModel.Books[indexPath.Row]);
            tableView.DeselectRow(indexPath, true);
        }
    }
}