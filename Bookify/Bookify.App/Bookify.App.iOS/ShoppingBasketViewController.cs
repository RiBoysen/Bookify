using Foundation;
using System;
using Bookify.App.Core.Initialization;
using Bookify.App.Core.Models;
using Bookify.App.Core.ViewModels;
using Bookify.App.iOS.Initialization;
using Bookify.App.iOS.Ui.Controllers.Base;
using UIKit;

namespace Bookify.App.iOS
{
    public partial class ShoppingBasketViewController : ExtendedViewController<ShoppingBasketViewModel>
    {
        public const string StoryboardIdentifier = "ShoppingBasketViewController";

        public ShoppingBasketViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.tblContent.Source = new ShoppingBasketDataSource(this.ViewModel);
            //this.tblContent.Delegate = new ShoppingBasketTableDelegate(this.ViewModel);
            this.tblContent.RowHeight = 100;
        }

        protected override void CreateBindings()
        {

        }
    }

    //public class ShoppingBasketTableDelegate : UITableViewDelegate
    //{
    //    private readonly ShoppingBasketViewModel viewModel;

    //    public ShoppingBasketTableDelegate(ShoppingBasketViewModel viewModel)
    //    {
    //        this.viewModel = viewModel;
    //    }

    //    public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
    //    {
    //        UITableViewRowAction btnRemove = UITableViewRowAction.Create(
    //            UITableViewRowActionStyle.Destructive,
    //            "Fjern �n",
    //            (action, path) =>
    //            {
    //                if (path.Section != 0)
    //                {
    //                    return;
    //                }
    //                var cartItem = this.viewModel.CartItems[indexPath.Row];
    //                if (cartItem.Quantity > 1)
    //                {
    //                    cartItem.Quantity--;
    //                    return;
    //                }

    //                tableView.BeginUpdates();
    //                this.viewModel.RemoveOne(cartItem);
    //                tableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Left);
    //                tableView.EndUpdates();
    //            });
    //        return new[] { btnRemove };
    //    }
    //}

    public class ShoppingBasketDataSource : UITableViewSource
    {
        private readonly ShoppingBasketViewModel viewModel;

        public ShoppingBasketDataSource(ShoppingBasketViewModel viewModel)
        {
            this.viewModel = viewModel;
        }


        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cartItem = this.viewModel.CartItems[indexPath.Row];
            return CartItemTableCell.CreateCell(tableView, indexPath, cartItem);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == 0)
            {
                return this.viewModel.CartItems.Count;
            }
            return 0;
        }
        
        public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewRowAction btnRemove = UITableViewRowAction.Create(
                UITableViewRowActionStyle.Destructive,
                "Fjern �n",
                (action, path) =>
                {
                    if (path.Section != 0)
                    {
                        return;
                    }
                    var cartItem = this.viewModel.CartItems[indexPath.Row];
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                        return;
                    }

                    tableView.BeginUpdates();
                    this.viewModel.RemoveOne(cartItem);
                    tableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Left);
                    tableView.EndUpdates();
                });
            return new[] { btnRemove };
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return indexPath.Section == 0;
        }
    }
}