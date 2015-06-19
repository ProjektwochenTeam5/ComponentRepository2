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
        /// <summary>
        /// The item to drag when clicked on.
        /// </summary>
        private MyCompControl movingItem = null;

        private MyCompControl selectedItem = null;

        private Link selectedLink = null;

        /// <summary>
        /// Only valid if movingItem != null.
        /// </summary>
        private Point movingItemInnerPosition;

        private ObservableCollection<Component> availableComps = new ObservableCollection<Component>();
        private List<MyCompControl> compLayout = new List<MyCompControl>();
        private List<Link> edgeLayout = new List<Link>();

        public MainWindow()
        {
            InitializeComponent();

            this.availableComps.Add(new Component()
            {
                FriendlyName = "Test-Komponente",
                InputHints = new string[] { "System.Int32", "System.String", "System.Int64", "System.Double", "System.Double" },
                InputDescriptions = new string[] { "int", "str", "long", "double", "double" },
                OutputHints = new string[] { "System.Int32", "System.Schiff" },
                OutputDescriptions = new string[] { "int", "schiff" }
            });

            this.Components_LB.ItemsSource = availableComps;

            this.availableComps.Add(new Component()
            {
                FriendlyName = "Input",
                OutputHints = new string[] { "System.String" },
                OutputDescriptions = new string[] { "string" },
            });

            this.availableComps.Add(new Component()
            {
                FriendlyName = "Output",
                InputHints = new string[] { "System.String" },
                InputDescriptions = new string[] { "string" },
            });

            this.availableComps.Add(new Component()
            {
                FriendlyName = "Schiff-Output",
                InputHints = new string[] { "System.Schiff", "System.String" },
                InputDescriptions = new string[] { "schiff", "string"},
            });

            this.availableComps.Add(new Component()
            {
                FriendlyName = "Schiff-Input",
                OutputHints = new string[] { "System.Schiff", "System.String" },
                OutputDescriptions = new string[] { "schiff", "string" },
            });

        }

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

        private void component_OnInputMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InputControl target = e.Source as InputControl;

            DeselectItem();
            DeselectLink();
            
            if (Link_ToB.IsChecked == false && target.IncomingLink != null)
            {
                this.selectedLink = target.IncomingLink;
                this.selectedLink.Line.Stroke = Brushes.Tomato;
            }
        }

        private void component_OnInputDragOver(object sender, DragEventArgs e)
        {
            InputControl target = e.Source as InputControl;
            e.Effects = DragDropEffects.None;

            // If in link-mode and no other link connects to the target input, then show that IN and OUT would fit.
            if (target != null && target.IncomingLink == null &&
                Link_ToB.IsChecked == true && e.Data.GetDataPresent(typeof(OutputControl)))
            {
                OutputControl src = (OutputControl)e.Data.GetData(typeof(OutputControl));

                if (target.ParentComponent != src.ParentComponent && target.Hint == src.Hint)
                {
                    e.Effects = DragDropEffects.Link;
                }
            }
        }

        private void component_OnInputDrop(object sender, DragEventArgs e)
        {
            InputControl target = e.Source as InputControl;

            // If in link-mode and no other link connects to the target input, then link IN with OUT if they fit.
            if (target != null && target.IncomingLink == null &&
                Link_ToB.IsChecked == true && e.Data.GetDataPresent(typeof(OutputControl)))
            {
                OutputControl src = (OutputControl)e.Data.GetData(typeof(OutputControl));

                if (target.ParentComponent != src.ParentComponent && target.Hint == src.Hint)
                {
                    Line line = new Line();

                    line.X1 = Canvas.GetLeft(src.ParentComponent) + src.ParentComponent.ActualWidth - 25;
                    line.Y1 = Canvas.GetTop(src.ParentComponent) + ((src.OutputValueID - 1) * 30) + 7;
                    line.X2 = Canvas.GetLeft(target.ParentComponent) + 20;
                    line.Y2 = Canvas.GetTop(target.ParentComponent) + ((target.InputValueID - 1) * 30) + 7;
                    line.Stroke = Brushes.White;
                    
                    Link link = new Link(src, target, line);
                    this.edgeLayout.Add(link);
                    ComponentCanvas.Children.Add(line);
                }
            }
        }

        private void component_OnOutputMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OutputControl srcOutput = (OutputControl)e.Source;

            DeselectItem();
            DeselectLink();

            if (Link_ToB.IsChecked == true && srcOutput.OutgoingLink == null)
            {
                DragDrop.DoDragDrop(srcOutput, srcOutput, DragDropEffects.Link);
            }
            else if (Link_ToB.IsChecked == false)
            {
                this.selectedLink = srcOutput.OutgoingLink;

                if (this.selectedLink != null)
                {
                    this.selectedLink.Line.Stroke = Brushes.Tomato;
                }
            }
        }

        private void component_OnBGMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeselectLink();
            DeselectItem();

            if (Link_ToB.IsChecked == false)
            {
                movingItem = (MyCompControl)sender;
                movingItemInnerPosition = e.GetPosition(movingItem);

                SelectItem(movingItem);
            }
        }

        private void component_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            movingItem = null;
        }

        private void DeselectLink()
        {
            if (this.selectedLink != null)
            {
                this.selectedLink.Line.Stroke = Brushes.White;
                this.selectedLink = null;
            }
        }

        private void SelectItem(MyCompControl item)
        {
            this.selectedItem = item;
            this.selectedItem.IsSelected = true;

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
        }

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

            this.selectedItem.IsSelected = false;
            this.selectedItem = null;
        }

        private void DeleteSelectedLink()
        {
            if (this.selectedLink != null)
            {
                this.edgeLayout.Remove(this.selectedLink);
                ComponentCanvas.Children.Remove(this.selectedLink.Line);
                this.selectedLink.Source.OutgoingLink = null;
                this.selectedLink.Target.IncomingLink = null;
                this.selectedLink = null;
            }
        }

        private void DeleteSelectedItem()
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
                        this.edgeLayout.Remove(input.IncomingLink);
                        this.ComponentCanvas.Children.Remove(input.IncomingLink.Line);
                        input.IncomingLink.Source.OutgoingLink = null;
                        input.IncomingLink = null;
                    }
                }
            }


            if (this.selectedItem.Outputs != null)
            {
                foreach (OutputControl output in this.selectedItem.Outputs)
                {
                    if (output.OutgoingLink != null)
                    {
                        this.edgeLayout.Remove(output.OutgoingLink);
                        this.ComponentCanvas.Children.Remove(output.OutgoingLink.Line);
                        output.OutgoingLink.Target.IncomingLink = null;
                        output.OutgoingLink = null;
                    }
                }
            }

            this.compLayout.Remove(this.selectedItem);
            this.ComponentCanvas.Children.Remove(this.selectedItem);
            this.selectedItem = null;
        }

        private void DeleteAll()
        {
            this.compLayout.Clear();
            this.edgeLayout.Clear();
            this.ComponentCanvas.Children.Clear();
            this.selectedItem = null;
            this.selectedLink = null;
        }
        
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

        private void Delete_B_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedLink();
            DeleteSelectedItem();
        }

        private void Components_LB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MyCompControl newComponent = null;

            if (Components_LB.SelectedItem != null)
            {
                newComponent = new MyCompControl((Component)Components_LB.SelectedItem);
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

        private void Link_ToB_Checked(object sender, RoutedEventArgs e)
        {
            DeselectLink();
            DeselectItem();
        }

        private void DeleteAll_B_Click(object sender, RoutedEventArgs e)
        {
            DeleteAll();
        }

        private void CreateJob_B_Click(object sender, RoutedEventArgs e)
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

            Component job = ConvertLayoutToComponent();

            //TODO job
            MessageBox.Show("Job is executing...",
                "Execute job",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private Component ConvertLayoutToComponent()
        {
            Component combinedComp = new Component();
            List<ComponentEdge> edges = new List<ComponentEdge>();
            List<string> inputHints = new List<string>();
            List<string> inputDescs = new List<string>();
            List<string> outputHints = new List<string>();
            List<string> outputDescs = new List<string>();

            foreach (Link link in edgeLayout)
            {
                ComponentEdge edge = new ComponentEdge();

                edge.OutputComponentGuid = link.Source.ParentComponent.Component.ComponentGuid;
                edge.InputComponentGuid = link.Target.ParentComponent.Component.ComponentGuid;
                edge.InputValueID = link.Target.InputValueID;
                edge.OutputValueID = link.Source.OutputValueID;
                edge.InternalInputComponentGuid = link.Target.ParentComponent.InternalComponentGuid;
                edge.InternalOutputComponentGuid = link.Source.ParentComponent.InternalComponentGuid;

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

        private void CreateComp_B_Click(object sender, RoutedEventArgs e)
        {
            if (this.edgeLayout.Count > 0)
            {
                ComponentCreateWindow createDlg = new ComponentCreateWindow(this.availableComps);
                createDlg.Owner = this;

                if (createDlg.ShowDialog() == true)
                {
                    Component newComp = ConvertLayoutToComponent();
                    newComp.FriendlyName = createDlg.FriendlyName;

                    this.availableComps.Add(newComp);

                    if (MessageBox.Show(
                            "Do you wish to clear the blueprint space?", 
                            "Clear blueprint space", 
                            MessageBoxButton.YesNo, 
                            MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        this.Link_ToB.IsChecked = false;
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
    }
}
