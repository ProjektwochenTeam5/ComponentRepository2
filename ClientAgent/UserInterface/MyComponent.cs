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

        /// <summary>
        /// Notifies this <see cref="MyComponent"/> instance of a new selected component
        /// which is used to set this instance's HasMatching... properties.
        /// </summary>
        /// <param name="component">The selected component.</param>
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

        /// <summary>
        /// Gets the underlying <see cref="Core.Network.Component"/> instance.
        /// </summary>
        public Core.Network.Component Component
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MyComponent"/> instance is marked as favorite.
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether this <see cref="MyComponent"/> instance has an input which matches
        /// at least one of the outputs of the selected component (set by SetSelectedComponent method).
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether this <see cref="MyComponent"/> instance has an output which matches
        /// at least one of the input of the selected component (set by SetSelectedComponent method).
        /// </summary>
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
