using System;
using Core.Network;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    /// <summary>
    /// This class serves as a wrapper for the <see cref="Core.Network.Component"/> class.
    /// Adds the functionality to set objects of this type as a favorite.
    /// </summary>
    public class MyComponent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isFavorite = false;
        private bool hasMatchingInput = false;
        private bool hasMatchingOutput = false;
        
        public MyComponent(Core.Network.Component childComponent)
        {
            this.Component = childComponent;
        }

        public void SetSelectedComponent(MyComponent component)
        {
            this.HasMatchingInput = false;
            this.HasMatchingOutput = false;

            if (component == null)
            {
                return;
            }

            if (component.Component.OutputHints != null && this.Component.InputHints != null)
            {
                foreach (string output in component.Component.OutputHints)
                {
                    if (this.Component.InputHints.Contains(output))
                    {
                        this.HasMatchingInput = true;
                        break;
                    }
                }
            }

            if (component.Component.InputHints != null && this.Component.OutputHints != null)
            {
                foreach (string input in component.Component.InputHints)
                {
                    if (this.Component.OutputHints.Contains(input))
                    {
                        this.HasMatchingOutput = true;
                        break;
                    }
                }
            }
        }

        public Core.Network.Component Component
        {
            get;
            private set;
        }

        public bool IsFavorite
        {
            get
            {
                return this.isFavorite;
            }
            set
            {
                this.isFavorite = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("IsFavorite"));
                }
            }
        }

        public bool HasMatchingInput
        {
            get
            {
                return this.hasMatchingInput;
            }
            private set
            {
                this.hasMatchingInput = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("HasMatchingInput"));
                }
            }
        }

        public bool HasMatchingOutput
        {
            get
            {
                return this.hasMatchingOutput;
            }
            private set
            {
                this.hasMatchingOutput = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("HasMatchingOutput"));
                }
            }
        }
    }
}
