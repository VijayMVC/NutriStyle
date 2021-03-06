﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DynamicConnections.NutriStyle.MenuGenerator.Controls.GridHelper;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting.Primitives;
using System.Xml.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Windows.Browser;
using DynamicConnections.NutriStyle.MenuGenerator.ChildWindows;
using DynamicConnections.NutriStyle.MenuGenerator.Pages;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{
    public partial class AdditionalProfiles : UserControl
    {
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();

        public AdditionalProfiles()
        {
            InitializeComponent();
            //dataGrid1.LoadingRow -= new EventHandler<DataGridRowEventArgs>(dataGrid_LoadingRow);

            if (((App)App.Current).GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
            {
                dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid_LoadingRow);
                dataGrid1.UnloadingRow -= new EventHandler<DataGridRowEventArgs>(dataGrid_LoadingRow);
                name.Content = ((App)App.Current).contact.Email;
                RetrieveAdditionalProfiles(new Guid(General.ContactId()));
            }

        }


        /// <summary>
        /// Assign event handlers to the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (sender != null)
            {
                General.DataGrid_AttachEvent(dataGrid1, e, Delete_Click, "Del");
                General.DataGrid_AttachEvent(dataGrid1, e, Edit_Click, "Edit");
                General.DataGrid_AttachEvent(dataGrid1, e, ShoppingList_Click, typeof(CheckBox));
            }
        }
        void Edit_Click(object sender, RoutedEventArgs e)
        {
            General g = new General();
            String Id = (String)((Row)dataGrid1.SelectedItem)["contactid"];
            ChildWindows.EditProfile ep = new EditProfile(new Guid(Id), new Guid(General.ContactId()));
            ep.SavedClicked += ep_SavedClicked;
            ep.Show();
        }
        void ShoppingList_Click(object sender, RoutedEventArgs e)
        {
            General g = new General();
            bool isChecked = (bool)((Row)dataGrid1.SelectedItem)["dc_rollshoppinglisttoparent"];
            String Id = (String)((Row)dataGrid1.SelectedItem)["contactid"];
            //upate the contact

            XElement contactXml = new XElement("contact");

            contactXml.Add(new XElement("contactid", Id));
            contactXml.Add(new XElement("dc_rollshoppinglisttoparent", isChecked.ToString()));
            contactXml.Add(new XElement("dc_createmenu", isChecked.ToString()));

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.CreateUpdateReturnEntityCompleted += new EventHandler<CrmSdk.CreateUpdateReturnEntityCompletedEventArgs>(cms_CreateUpdateContactShoppingList);
            cms.CreateUpdateReturnEntityAsync(contactXml, true);

        }
        private void cms_CreateUpdateContactShoppingList(object sender, CrmSdk.CreateUpdateReturnEntityCompletedEventArgs e)
        {
            var errors = from x in e.Result.Descendants("error") select x;
            if (errors.Count() > 0)
            {
                String message = e.Result.Value.ToString();
                Status s = new Status(message, false);
                s.Show();
            }
        }

        void ep_SavedClicked(object sender, EventArgs e)
        {
            XElement element = (XElement)sender;

            var lastname            = String.Empty;
            var firstname           = String.Empty;
            var rollupShoppingList  = false;

            if (element.Descendants("contact").First().Descendants("firstname").Any())
            {
                firstname = element.Descendants("contact").First().Descendants("firstname").First().Value;
            }

            if (element.Descendants("contact").First().Descendants("lastname").Any())
            {
                lastname = element.Descendants("contact").First().Descendants("lastname").First().Value;
            }
            if (element.Descendants("contact").First().Descendants("dc_rollshoppinglisttoparent").Any())
            {
                Boolean.TryParse(element.Descendants("contact").First().Descendants("dc_rollshoppinglisttoparent").First().Value, out rollupShoppingList);
            }
            var contactId = element.Descendants("contact").First().Descendants("contactid").First().Value;

            SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;

            var node = from x in data where (string)x["contactid"] == contactId select x;
            Row row = ((Row)node.FirstOrDefault());
            if (row == null)
            {//new record
                row = new Row();
                //add needed fields
                row["firstname"] = firstname;
                row["lastname"] = lastname;
                row["contactid"] = contactId;
                row["dc_rollshoppinglisttoparent"] = rollupShoppingList;
                data.Add(row);
            }
            else
            {
                row["firstname"] = firstname;
                row["lastname"] = lastname;
                row["dc_rollshoppinglisttoparent"] = rollupShoppingList;
            }
        }

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            General g = new General();
            String Id = (String)((Row)dataGrid1.SelectedItem)["contactid"];
            Row r = (Row)dataGrid1.SelectedItem;
            g.Delete("contact", new Guid(Id));
            g.DeleteCompleted += g_DeleteClicked;
        }

        void g_DeleteClicked(object sender, EventArgs e)
        {
            var element = (XElement)sender;
            //get Id and remove from grid
            String Id = element.FirstNode.ToString();
            //Find on grid
            SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;

            var node = from x in data where (string)x["contactid"] == Id select x;
            Row row = ((Row)node.FirstOrDefault());
            if (row != null)
            {
                data.Remove(row);
            }
        }

        public void RetrieveAdditionalProfiles(Guid contactId)
        {
            busyIndicator.IsBusy = true;
            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.RetrieveAdditionalProfilesCompleted += new EventHandler<CrmSdk.RetrieveAdditionalProfilesCompletedEventArgs>(cms_CreateUpdateGrid);
            cms.RetrieveAdditionalProfilesAsync(contactId.ToString());
        }


        private void cms_CreateUpdateGrid(object sender, CrmSdk.RetrieveAdditionalProfilesCompletedEventArgs e)
        {
            SortableCollectionView data = new SortableCollectionView();
            var element = e.Result;
            DataGrid dataGrid = dataGrid1;
            String entityName = "contact";
            try
            {
                if (element.Descendants("columns").Count() > 0)
                {
                    XElement xEl = element.Descendants("columns").ToList()[0];//get first one
                    //MessageBox.Show(xEl.ToString());
                    foreach (XElement xe in xEl.Elements())
                    {

                        data.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);

                        if (!xe.Name.LocalName.Equals("dc_rollshoppinglisttoparent", StringComparison.OrdinalIgnoreCase))
                        {
                            DataGridTextColumn dg = new DataGridTextColumn();
                            dg.Header = xe.Attribute("LabelName").Value;
                            dg.CanUserSort = true;
                            dg.SortMemberPath = xe.Name.LocalName;
                            dg.Binding = new Binding("Data")
                                {
                                    Converter = _rowIndexConverter,
                                    ConverterParameter = xe.Name.LocalName
                                };

                            //visable?
                            dg.Visibility = System.Windows.Visibility.Visible;
                            if (xe.Name.LocalName.Equals(entityName + "id", StringComparison.OrdinalIgnoreCase))
                            {
                                dg.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            dg.IsReadOnly = true;
                            dataGrid.Columns.Add(dg);

                        }

                    }
                    //Add shopping list icon
                    DataGridTemplateColumn checkbox = Columns.CheckBox("Add to the Primary Shopping List", "shippingListCheckbox", "dc_rollshoppinglisttoparent");
                    checkbox.HeaderStyle = (Style)App.Current.Resources["DataGridColumnHeaderShoppingListIcon"];
                    checkbox.CanUserResize = false;
                    dataGrid.Columns.Add(checkbox);
                    //add edit button
                    dataGrid.Columns.Add(Controls.GridHelper.Columns.Button("Edit", "Edit", "Edit"));

                    //add image for delete
                    dataGrid.Columns.Add(Controls.GridHelper.Columns.Delete("Remove this Profile"));

                    //Bind data
                    IEnumerable<XElement> rows = element.Descendants(entityName);
                    data = Controls.GridHelper.Data.Attach(rows, data);
                }

                dataGrid.ItemsSource = data;

                dataGrid.Visibility = System.Windows.Visibility.Visible;
                dataGrid.SelectedIndex = 0;

                dataGrid.BorderThickness = new Thickness(1);

                // Space available to fill ( -18 Standard vScrollbar)
                double space_available = (LayoutRoot.ActualWidth - 48 - 50 - 26); //18 is width of scroll bar, 150 is width of menu 
                //figure out column types

                int count = 4;
                if (space_available > 0)
                {
                    foreach (DataGridColumn dg_c in dataGrid1.Columns)
                    {
                        //Do not adjust the delete column
                        if (!dg_c.Header.ToString().Equals("del", StringComparison.OrdinalIgnoreCase) &&
                            !dg_c.Header.ToString().Equals("edit", StringComparison.OrdinalIgnoreCase) &&
                            dg_c.Width.DesiredValue != 26)
                        {
                            dg_c.Width = new DataGridLength((space_available / (dataGrid1.Columns.Count - count)));
                        }

                    }
                }
            }
            catch (Exception err)
            {
                ChildWindows.Error eWindow = new Error(err.Message + "\n" + err.StackTrace);
                eWindow.Show();
            }
            busyIndicator.IsBusy = false;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //RetrieveAdditionalProfiles(new Guid(General.ContactId()));
        }
        /// <summary>
        /// Open profile window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newProfile_Click(object sender, RoutedEventArgs e)
        {
            ChildWindows.EditProfile ep = new EditProfile(Guid.Empty, new Guid(General.ContactId()));
            ep.SavedClicked += ep_SavedClicked;
            ep.Show();//need to pass in the parent contactId

        }
    }
}

