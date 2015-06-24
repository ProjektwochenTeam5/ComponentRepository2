using Core.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MyComponent STATIC_STRING_COMPONENT =
            new MyComponent(
                new Component()
                {
                    FriendlyName = "STATIC STRING",
                    OutputHints = new string[] { "System.String" },
                    OutputDescriptions = new string[] { "out" }
                });

        /// <summary>
        /// The component to drag when clicked on.
        /// </summary>
        private MyCompControl movingItem = null;

        /// <summary>
        /// The component selected by the user.
        /// </summary>
        private MyCompControl selectedItem = null;

        /// <summary>
        /// The link selected by the user.
        /// </summary>
        private Link selectedLink = null;

        /// <summary>
        /// The mouse position relative to the moving item.
        /// Only valid if movingItem != null.
        /// </summary>
        private Point movingItemInnerPosition;

        /// <summary>
        /// The collection containing all available components.
        /// </summary>
        private ObservableCollection<MyComponent> availableComps = new ObservableCollection<MyComponent>();

        /// <summary>
        /// The collection containing the components which are set as favorites by the user.
        /// </summary>
        private ObservableCollection<MyComponent> favorites = new ObservableCollection<MyComponent>();

        /// <summary>
        /// The collection containing all components which have an input hint that matches the any output of the selected item.
        /// Is empty if no component is selected.
        /// </summary>
        private ObservableCollection<MyComponent> matchingIn = new ObservableCollection<MyComponent>();

        /// <summary>
        /// The collection containing all components which have an output hint that matches the any input of the selected item.
        /// Is empty if no component is selected.
        /// </summary>
        private ObservableCollection<MyComponent> matchingOut = new ObservableCollection<MyComponent>();

        /// <summary>
        /// The input identifiers of the input components along with their input descriptions.
        /// </summary>
        private Dictionary<InputIdentifier, string> inputDescriptions = new Dictionary<InputIdentifier, string>();

        /// <summary>
        /// The list containing all components which are visible on the canvas.
        /// </summary>
        private List<MyCompControl> compLayout = new List<MyCompControl>();

        /// <summary>
        /// The list containing all edges which make up the job.
        /// </summary>
        private List<Link> edgeLayout = new List<Link>();

        /// <summary>
        /// The list containing all edges which are only used in the user interface to connect static strings and string inputs.
        /// </summary>
        private List<Link> staticStringLinks = new List<Link>();

        public MainWindow()
        {
            /*
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Atomic component|*.dll";
            dlg.ShowDialog();
            */

            InitializeComponent();

            this.lvComponents.DataContext = availableComps;
            this.lvFavorites.DataContext = favorites;
            this.lvMatchingIn.DataContext = matchingIn;
            this.lvMatchingOut.DataContext = matchingOut;

            #region Test components TODO
            this.availableComps.Add(new MyComponent(new Component()
            {
                FriendlyName = "Test-Komponente",
                InputHints = new string[] { "System.Int32", "System.String", "System.Int64", "System.Double", "System.String" },
                InputDescriptions = new string[] { "int", "str", "long", "double", "str2" },
                OutputHints = new string[] { "System.Int32", "System.Schiff" },
                OutputDescriptions = new string[] { "int", "schiff" }
            }));
            
            this.availableComps.Add(new MyComponent(new Component()
            {
                FriendlyName = "Input",
                InputHints = new string[] { "System.String" },
                InputDescriptions = new string[] { "descr" },
                OutputHints = new string[] { "System.String" },
                OutputDescriptions = new string[] { "string" },
            }));

            this.availableComps.Add(new MyComponent(new Component()
            {
                FriendlyName = "Output",
                InputHints = new string[] { "System.String" },
                InputDescriptions = new string[] { "string" },
            }));

            this.availableComps.Add(new MyComponent(new Component()
            {
                FriendlyName = "Schiff-Output",
                InputHints = new string[] { "System.Schiff", "System.String" },
                InputDescriptions = new string[] { "schiff", "string"},
            }));

            this.availableComps.Add(new MyComponent(new Component()
            {
                FriendlyName = "Schiff-Input",
                OutputHints = new string[] { "System.Schiff", "System.String" },
                OutputDescriptions = new string[] { "schiff", "string" },
            }));
            #endregion Test components
        }

        /// <summary>
        /// If there is a moving item, moves it on the canvas and all connected lines.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComponentCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // If there is a moving item, drag it and its connected lines
            if (movingItem != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Vector newPosition = e.GetPosition(ComponentCanvas) - movingItemInnerPosition;
                bool leftChanged = false;
                bool topChanged = false;

                // move item horizontal if inside canvas
                if (newPosition.X > 0 && newPosition.X + movingItem.RenderSize.Width < ComponentCanvas.ActualWidth)
                {
                    Canvas.SetLeft(movingItem, newPosition.X);
                    leftChanged = true;
                }

                // move item vertical if inside canvas
                if (newPosition.Y > 0 && newPosition.Y + movingItem.RenderSize.Height < ComponentCanvas.ActualHeight)
                {
                    Canvas.SetTop(movingItem, newPosition.Y);
                    topChanged = true;
                }

                // move the lines connected to the inputs of the moving item
                foreach (InputControl input in movingItem.Inputs)
                {
                    if (input.IncomingLink != null)
                    {
                        if (leftChanged)
                        {
                            input.IncomingLink.Line.X2 = newPosition.X + 20;
                        }

                        if (topChanged)
                        {
                            input.IncomingLink.Line.Y2 = newPosition.Y + ((input.InputValueID - 1) * 30) + 7;
                        }
                    }
                }

                // move the lines connected to the outputs of the moving item
                foreach (OutputControl output in movingItem.Outputs)
                {
                    if (output.OutgoingLink != null)
                    {
                        if (leftChanged)
                        {
                            output.OutgoingLink.Line.X1 = newPosition.X + movingItem.ActualWidth - 25;
                        }

                        if (topChanged)
                        {
                            output.OutgoingLink.Line.Y1 = newPosition.Y + ((output.OutputValueID - 1) * 30) + 7;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If toggle link is not checked sets the connected link as the selected link and highlights it if it exists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void component_OnInputMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InputControl target = e.Source as InputControl;

            DeselectItem();
            DeselectLink();
            
            if (toggleLink.IsChecked == false && target.IncomingLink != null)
            {
                this.selectedLink = target.IncomingLink;
                this.selectedLink.Line.Stroke = Brushes.Tomato;
            }
        }

        /// <summary>
        /// If toggle link is checked and the input hints match, sets the drop effects to link.
        /// If the input hints do not match, sets the drop effects to none.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void component_OnInputDragOver(object sender, DragEventArgs e)
        {
            InputControl target = e.Source as InputControl;
            e.Effects = DragDropEffects.None;

            // If in link-mode and no other link connects to the target input, then show that IN and OUT would fit.
            if (target != null && target.IncomingLink == null &&
                toggleLink.IsChecked == true && e.Data.GetDataPresent(typeof(OutputControl)))
            {
                OutputControl src = (OutputControl)e.Data.GetData(typeof(OutputControl));

                if (target.ParentControl != src.ParentControl && target.Hint == src.Hint)
                {
                    e.Effects = DragDropEffects.Link;
                }
            }
        }
      
        /// <summary>
        /// If toggle link is checked and the input hints match, connects the drag source output to the input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void component_OnInputDrop(object sender, DragEventArgs e)
        {
            InputControl target = e.Source as InputControl;

            // If in link-mode and no other link connects to the target input, then link IN with OUT if they fit.
            if (target != null && target.IncomingLink == null &&
                toggleLink.IsChecked == true && e.Data.GetDataPresent(typeof(OutputControl)))
            {
                OutputControl src = (OutputControl)e.Data.GetData(typeof(OutputControl));

                if (target.ParentControl != src.ParentControl && target.Hint == src.Hint)
                {
                    Line line = new Line();

                    line.X1 = Canvas.GetLeft(src.ParentControl) + src.ParentControl.ActualWidth - 25;
                    line.Y1 = Canvas.GetTop(src.ParentControl) + ((src.OutputValueID - 1) * 30) + 7;
                    line.X2 = Canvas.GetLeft(target.ParentControl) + 20;
                    line.Y2 = Canvas.GetTop(target.ParentControl) + ((target.InputValueID - 1) * 30) + 7;
                    line.Stroke = Brushes.White;
                    
                    if (src.ParentControl.Component == this.STATIC_STRING_COMPONENT)
                    {
                        ComponentCanvas.Children.Add(line);

                        StaticStringInputWindow dlg = new StaticStringInputWindow();
                        dlg.Owner = this;

                        if (dlg.ShowDialog() == true)
                        {
                            this.inputDescriptions.Add(new InputIdentifier(target.ParentControl.InternalComponentGuid, target.InputValueID), dlg.Description);

                            Link staticInputLink = new Link(src, target, line);
                            this.edgeLayout.Add(staticInputLink);
                            this.staticStringLinks.Add(staticInputLink);
                        }
                        else
                        {
                            ComponentCanvas.Children.Remove(line);
                        }
                    }
                    else
                    {
                        ComponentCanvas.Children.Add(line);

                        Link link = new Link(src, target, line);
                        this.edgeLayout.Add(link);
                    }
                }
            }
        }

        /// <summary>
        /// If toggle link is checked and the pressed output is not already connected, sets the output as the drag source.
        /// If toggle link is not checked sets the selected link and highlights it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void component_OnOutputMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OutputControl srcOutput = (OutputControl)e.Source;

            DeselectItem();
            DeselectLink();

            if (toggleLink.IsChecked == true && srcOutput.OutgoingLink == null)
            {
                DragDrop.DoDragDrop(srcOutput, srcOutput, DragDropEffects.Link);
            }
            else if (toggleLink.IsChecked == false)
            {
                this.selectedLink = srcOutput.OutgoingLink;

                if (this.selectedLink != null)
                {
                    this.selectedLink.Line.Stroke = Brushes.Tomato;
                }
            }
        }

        /// <summary>
        /// If toggle link is not checked, sets the clicked component as the selected item.
        /// </summary>
        /// <param name="sender">The <see cref="MyCompControl"/> instance which was clicked.</param>
        /// <param name="e"></param>
        private void component_OnBGMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeselectLink();
            DeselectItem();

            if (toggleLink.IsChecked == false)
            {
                movingItem = (MyCompControl)sender;
                movingItemInnerPosition = e.GetPosition(movingItem);

                SelectItem(movingItem);
            }
        }

        /// <summary>
        /// Sets the moving item to null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void component_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            movingItem = null;
        }

        /// <summary>
        /// Sets the given item as the selected item and highlights all connecting lines.
        /// Updates the matching input and matching output lists.
        /// </summary>
        /// <param name="item">The new selected item.</param>
        private void SelectItem(MyCompControl item)
        {
            this.selectedItem = item;
            this.selectedItem.IsSelected = true;

            // Highlight connecting lines
            if (this.selectedItem.Inputs != null)
            {
                foreach (InputControl input in this.selectedItem.Inputs)
                {
                    if (input.IncomingLink != null)
                    {
                        input.IncomingLink.Line.Stroke = Brushes.Tomato;
                    }
                }
            }

            if (this.selectedItem.Outputs != null)
            {
                foreach (OutputControl output in this.selectedItem.Outputs)
                {
                    if (output.OutgoingLink != null)
                    {
                        output.OutgoingLink.Line.Stroke = Brushes.Tomato;
                    }
                }
            }

            // update matching inputs and outputs
            this.matchingIn.Clear();
            this.matchingOut.Clear();

            foreach (MyComponent c in this.availableComps)
            {
                c.SetSelectedComponent(selectedItem.Component);
                
                if (c.HasMatchingInput)
                {
                    this.matchingIn.Add(c);
                }

                if (c.HasMatchingOutput)
                {
                    this.matchingOut.Add(c);
                }
            }
        }

        /// <summary>
        /// Sets the selected item to null.
        /// Resets the color of the highlighted connecting lines.
        /// </summary>
        private void DeselectItem()
        {
            if (this.selectedItem == null)
            {
                return;
            }

            if (this.selectedItem.Inputs != null)
            {
                foreach (InputControl input in this.selectedItem.Inputs)
                {
                    if (input.IncomingLink != null)
                    {
                        input.IncomingLink.Line.Stroke = Brushes.White;
                    }
                }
            }

            if (this.selectedItem.Outputs != null)
            {
                foreach (OutputControl output in this.selectedItem.Outputs)
                {
                    if (output.OutgoingLink != null)
                    {
                        output.OutgoingLink.Line.Stroke = Brushes.White;
                    }
                }
            }

            // reset HasMatching...-Properties of components
            ResetMatchingItems();

            this.selectedItem.IsSelected = false;
            this.selectedItem = null;
        }

        /// <summary>
        /// Sets the selected link to null.
        /// Resets the color of the line.
        /// </summary>
        private void DeselectLink()
        {
            if (this.selectedLink != null)
            {
                this.selectedLink.Line.Stroke = Brushes.White;
                this.selectedLink = null;
            }
        }

        /// <summary>
        /// Removes the selected link from the canvas and the edgeLayout.
        /// Resets the links of connected components.
        /// </summary>
        private void DeleteSelectedLink()
        {
            if (this.selectedLink != null)
            {
                if (selectedLink.Source.ParentControl.Component == this.STATIC_STRING_COMPONENT)
                {
                    this.inputDescriptions.Remove(
                        new InputIdentifier(
                            this.selectedLink.Target.ParentControl.InternalComponentGuid, 
                            this.selectedLink.Target.InputValueID));
                    this.staticStringLinks.Remove(this.selectedLink);
                }

                this.edgeLayout.Remove(this.selectedLink);
                ComponentCanvas.Children.Remove(this.selectedLink.Line);
                this.selectedLink.Source.OutgoingLink = null;
                this.selectedLink.Target.IncomingLink = null;
                this.selectedLink = null;
            }
        }

        /// <summary>
        /// Removes the selected item and all connected links from the canvas and the edgeLayout.
        /// Resets the links of connected components.
        /// </summary>
        private void DeleteSelectedItem()
        {
            if (this.selectedItem == null)
            {
                return;
            }

            bool isStaticStringComponent = this.selectedItem.Component == STATIC_STRING_COMPONENT;

            // Remove incoming links
            if (this.selectedItem.Inputs != null)
            {
                if (this.selectedItem.Component == STATIC_STRING_COMPONENT && this.selectedItem.Outputs[0].OutgoingLink != null)
                {
                    this.inputDescriptions.Remove(
                        new InputIdentifier(
                            this.selectedItem.InternalComponentGuid,
                            this.selectedItem.Outputs[0].OutgoingLink.Target.InputValueID));
                }

                foreach (InputControl input in this.selectedItem.Inputs)
                {
                    if (input.IncomingLink != null)
                    {
                        if (input.IncomingLink.Source.ParentControl.Component == STATIC_STRING_COMPONENT)
                        {
                            this.inputDescriptions.Remove(
                                new InputIdentifier(
                                    this.selectedItem.InternalComponentGuid,
                                    input.IncomingLink.Target.InputValueID));
                        }

                        this.edgeLayout.Remove(input.IncomingLink);
                        this.ComponentCanvas.Children.Remove(input.IncomingLink.Line);
                        input.IncomingLink.Source.OutgoingLink = null;
                        input.IncomingLink = null;
                    }
                }
            }

            // Remove outgoing links
            if (this.selectedItem.Outputs != null)
            {
                foreach (OutputControl output in this.selectedItem.Outputs)
                {
                    if (output.OutgoingLink != null)
                    {
                        if (isStaticStringComponent)
                        {
                            staticStringLinks.Remove(output.OutgoingLink);
                        }

                        this.edgeLayout.Remove(output.OutgoingLink);
                        this.ComponentCanvas.Children.Remove(output.OutgoingLink.Line);
                        output.OutgoingLink.Target.IncomingLink = null;
                        output.OutgoingLink = null;
                    }
                }
            }

            // reset HasMatching...-Properties of components
            ResetMatchingItems();

            this.compLayout.Remove(this.selectedItem);
            this.ComponentCanvas.Children.Remove(this.selectedItem);
            this.selectedItem = null;
        }

        /// <summary>
        /// Removes all items from the canvas and clears compLayout and edgeLayout.
        /// </summary>
        private void DeleteAll()
        {
            this.compLayout.Clear();
            this.edgeLayout.Clear();
            this.inputDescriptions.Clear();
            this.staticStringLinks.Clear();
            this.ComponentCanvas.Children.Clear();

            this.selectedItem = null;
            this.selectedLink = null;

            // reset HasMatching...-Properties of components
            ResetMatchingItems();
        }

        /// <summary>
        /// Clears the matching input and matching output lists and
        /// resets the HasMatching... properties of all available components.
        /// </summary>
        private void ResetMatchingItems()
        {
            this.matchingIn.Clear();
            this.matchingOut.Clear();

            foreach (MyComponent c in this.availableComps)
            {
                c.SetSelectedComponent(null);
            }
        }
                
        /// <summary>
        /// Deletes the selected item or link when Delete is pressed.
        /// Deselects the selected item or link when Escape is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    DeleteSelectedLink();
                    DeleteSelectedItem();
                    break;
                case Key.Escape:
                    DeselectLink();
                    DeselectItem();
                    break;
            }
        }

        /// <summary>
        /// Deletes the selected item or link.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedLink();
            DeleteSelectedItem();
        }

        /// <summary>
        /// Adds the selected component to the layout on the canvas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComponentsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView senderView = (ListView)sender;
            MyCompControl newComponent = null;

            if (senderView.SelectedItem != null)
            {
                newComponent = new MyCompControl((MyComponent)senderView.SelectedItem);
                newComponent.AllowDrop = true;
                newComponent.OnBGMouseLeftButtonDown += component_OnBGMouseLeftButtonDown;
                newComponent.MouseLeftButtonUp += component_MouseLeftButtonUp;
                newComponent.OnInputMouseLeftButtonDown += component_OnInputMouseLeftButtonDown;
                newComponent.OnInputDragOver += component_OnInputDragOver;
                newComponent.OnInputDrop += component_OnInputDrop;
                newComponent.OnOutputMouseLeftButtonDown += component_OnOutputMouseLeftButtonDown;
                Canvas.SetZIndex(newComponent, 1); // move above connecting lines
                Canvas.SetLeft(newComponent, 10);
                Canvas.SetTop(newComponent, 10);
                
                compLayout.Add(newComponent);
                ComponentCanvas.Children.Add(newComponent);
            }
        }

        /// <summary>
        /// Deselects the selected link and the selected item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleLink_Checked(object sender, RoutedEventArgs e)
        {
            DeselectLink();
            DeselectItem();
        }

        /// <summary>
        /// Calls DeleteAll method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            DeleteAll();
        }

        /// <summary>
        /// Checks if all inputs and outputs are connected and converts the layout to a job.
        /// TODO do anything with the job.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateJob_Click(object sender, RoutedEventArgs e)
        {
            MyCompControl comp = null;
            bool everythingConnected = true;

            if (this.compLayout.Count == 0)
            {
                MessageBox.Show("You did not yet create an executable job,\nbecause there are no components.",
                    "Execute job",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // check if all inputs and outputs are connected
            for (int i = 0; everythingConnected && i < compLayout.Count; i++)
            {
                comp = compLayout[i];

                for (int k = 0; everythingConnected && k < comp.Inputs.Count; k++)
                {
                    if (comp.Inputs[k].IncomingLink == null)
                    {
                        everythingConnected = false;
                    }
                }

                for (int m = 0; everythingConnected && m < comp.Outputs.Count; m++)
                {
                    if (comp.Outputs[m].OutgoingLink == null)
                    {
                        everythingConnected = false;
                    }
                }
            }

            if (!everythingConnected)
            {
                MessageBox.Show("You did not yet create an executable job,\nbecause there exist unconnected ports.",
                    "Execute job",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            Job job = new Job();
            job.InputDescriptions = this.inputDescriptions;
            job.JobComponent = ConvertLayoutToComponent();

            //TODO job
            MessageBox.Show("Job is executing...",
                "Execute job",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /// <summary>
        /// Converts the current layout which is visible on the canvas to a component.
        /// </summary>
        /// <returns>The component defined by the edge layout.</returns>
        private Component ConvertLayoutToComponent()
        {
            Component combinedComp = new Component();
            List<ComponentEdge> edges = new List<ComponentEdge>();
            List<string> inputHints = new List<string>();
            List<string> inputDescs = new List<string>();
            List<string> outputHints = new List<string>();
            List<string> outputDescs = new List<string>();
            List<Link> usefulLinks = new List<Link>(edgeLayout);

            foreach (Link unusefulLink in staticStringLinks)
            {
                usefulLinks.Remove(unusefulLink);
            }

            foreach (Link link in usefulLinks)
            {
                ComponentEdge edge = new ComponentEdge();

                edge.OutputComponentGuid = link.Source.ParentControl.Component.Component.ComponentGuid;
                edge.InputComponentGuid = link.Target.ParentControl.Component.Component.ComponentGuid;
                edge.InputValueID = link.Target.InputValueID;
                edge.OutputValueID = link.Source.OutputValueID;
                edge.InternalInputComponentGuid = link.Target.ParentControl.InternalComponentGuid;
                edge.InternalOutputComponentGuid = link.Source.ParentControl.InternalComponentGuid;

                edges.Add(edge);
            }
            
            foreach (MyCompControl comp in compLayout)
            {
                foreach (InputControl input in comp.Inputs)
                {
                    if (input.IncomingLink == null)
                    {
                        inputHints.Add(input.Hint);
                        inputDescs.Add(input.Description);
                    }
                }

                foreach (OutputControl output in comp.Outputs)
                {
                    if (output.OutgoingLink == null)
                    {
                        outputHints.Add(output.Hint);
                        outputDescs.Add(output.Description);
                    }
                }
            }

            combinedComp.ComponentGuid = Guid.NewGuid();
            combinedComp.IsAtomic = false;
            combinedComp.Edges = edges;
            combinedComp.InputHints = inputHints;
            combinedComp.InputDescriptions = inputDescs;
            combinedComp.OutputHints = outputHints;
            combinedComp.OutputDescriptions = outputDescs;

            return combinedComp;
        }

        /// <summary>
        /// Adds a new component which contains the functionality of the visible layout of the canvas.
        /// Clears the canvas if the user wishes to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateComp_Click(object sender, RoutedEventArgs e)
        {
            if (this.edgeLayout.Count > 0)
            {
                ComponentCreateWindow createDlg = new ComponentCreateWindow(this.availableComps);
                createDlg.Owner = this;

                if (createDlg.ShowDialog() == true)
                {
                    Component newComp = ConvertLayoutToComponent();
                    newComp.FriendlyName = createDlg.FriendlyName;

                    this.availableComps.Add(new MyComponent(newComp));

                    if (MessageBox.Show(
                            "Do you wish to clear the blueprint space?", 
                            "Clear blueprint space", 
                            MessageBoxButton.YesNo, 
                            MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        this.toggleLink.IsChecked = false;
                        DeleteAll();
                    }
                }
            }
            else
            {
                MessageBox.Show("You did not yet create a component which is worth saving,\nbecause there are no edges.", 
                    "Create component", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Adds the associated component to the list of favorites if it is not already contained.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FavoriteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox s = (CheckBox)sender;
            MyComponent comp = ((ContentPresenter)s.TemplatedParent).Content as MyComponent;

            if (comp != null && !favorites.Contains(comp))
            {
                favorites.Add(comp);
            }
        }

        /// <summary>
        /// Removes the associated component from the list of favorites if it is contained.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FavoriteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox s = (CheckBox)sender;
            MyComponent comp = ((ContentPresenter)s.TemplatedParent).Content as MyComponent;

            if (comp != null && favorites.Contains(comp))
            {
                favorites.Remove(comp);
            }
        }

        private void btnStaticInput_Click(object sender, RoutedEventArgs e)
        {
            MyCompControl staticInputComponent = new MyCompControl(this.STATIC_STRING_COMPONENT);
            staticInputComponent.OnBGMouseLeftButtonDown += component_OnBGMouseLeftButtonDown;
            staticInputComponent.MouseDoubleClick += inputComponent_MouseDoubleClick;
            staticInputComponent.MouseLeftButtonUp += component_MouseLeftButtonUp;
            staticInputComponent.OnOutputMouseLeftButtonDown += component_OnOutputMouseLeftButtonDown;

            Canvas.SetZIndex(staticInputComponent, 1); // move above connecting lines
            Canvas.SetLeft(staticInputComponent, 10);
            Canvas.SetTop(staticInputComponent, 10);
            this.ComponentCanvas.Children.Add(staticInputComponent);
        }

        /// <summary>
        /// Enables the user to edit the string which was entered before.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputComponent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {            
            OutputControl descriptionOut = ((MyCompControl)sender).Outputs[0];

            if (descriptionOut.OutgoingLink != null)
            {
                InputControl target = descriptionOut.OutgoingLink.Target;
                StaticStringInputWindow dlg = new StaticStringInputWindow();
                dlg.Owner = this;
                
                if (dlg.ShowDialog() == true)
                {
                    InputIdentifier inputId = new InputIdentifier(
                        target.ParentControl.InternalComponentGuid, 
                        target.InputValueID);

                    this.inputDescriptions[inputId] = dlg.Description;
                }
            }
        }        
    }
}
