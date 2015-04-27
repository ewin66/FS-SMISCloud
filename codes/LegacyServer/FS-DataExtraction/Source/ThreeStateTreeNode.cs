using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace Ascentium.Research.Windows.Forms.Components
{
    /// <summary>
    /// ThreeStateTreeNode inherits from <see cref="http://msdn2.microsoft.com/en-us/library/system.windows.forms.treenode.aspx">TreeNode</see>
    /// and adds the ability to support a third, indeterminate state as well as optionally cascading state changes to related nodes, i.e.
    /// child nodes and or parent nodes, as determined by this instance's related parent TreeView settings, CascadeNodeChecksToChildNodes and
    /// CascadeNodeChecksToParentNode.
    /// </summary>
    public class ThreeStateTreeNode : TreeNode
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ThreeStateTreeNode class in addition to intializing
        /// the base class (<see cref="http://msdn2.microsoft.com/en-us/library/bk8h64c9.aspx">TreeNode Constructor</see>). 
        /// </summary>
        public ThreeStateTreeNode() : base()
        {
            this.CommonConstructor();
        }
        
        /// <summary>
        /// Initializes a new instance of the ThreeStateTreeNode class with a string for the text label to display in addition to intializing
        /// the base class (<see cref="http://msdn2.microsoft.com/en-us/library/ytx906df.aspx">TreeNode Constructor</see>). 
        /// </summary>
        /// <param name="text">The string for the label of the new tree node.</param>
        public ThreeStateTreeNode(string text) : base(text)
        {
            this.CommonConstructor();
        }

        /// <summary>
        /// Initializes a new instance of the ThreeStateTreeNode class with a string for the text label to display 
        /// and an array of child ThreeStateTreeNodes in addition to intializing the base class 
        /// (<see cref="http://msdn2.microsoft.com/en-us/library/774ty506.aspx">TreeNode Constructor</see>). 
        /// </summary>
        /// <param name="text">The string for the label of the new tree node.</param>
        /// <param name="children">An array of child ThreeStateTreeNodes.</param>
        public ThreeStateTreeNode(string text, ThreeStateTreeNode[] children) : base(text, children)
        {
            this.CommonConstructor();
        }
        
        /// <summary>
        /// Initializes a new instance of the ThreeStateTreeNode class with a string for the text label to display 
        /// and the selected and unselected image indexes in addition to intializing the base class 
        /// (<see cref="http://msdn2.microsoft.com/en-us/library/8dfy3k5t.aspx">TreeNode Constructor</see>). 
        /// </summary>
        /// <param name="text">The string for the label of the new tree node.</param>
        /// <param name="imageIndex">The image index of the unselected image in the parent TreeView's <see cref="http://msdn2.microsoft.com/en-us/library/system.windows.forms.treeview.imagelist.aspx">ImageList</see>.</param>
        /// <param name="selectedImageIndex">The image index of the selected image in the parent TreeView's <see cref="http://msdn2.microsoft.com/en-us/library/system.windows.forms.treeview.imagelist.aspx">ImageList</see>.</param>
        public ThreeStateTreeNode(string text, int imageIndex, int selectedImageIndex) : base(text, imageIndex, selectedImageIndex)
        {
            this.CommonConstructor(); 
        }

        /// <summary>
        /// Initializes a new instance of the ThreeStateTreeNode class with a string for the text label to display ,
        /// the selected and unselected image indexes, and an array of child ThreeStateTreeNodes in addition to intializing the base class 
        /// (<see cref="http://msdn2.microsoft.com/en-us/library/8dfy3k5t.aspx">TreeNode Constructor</see>). 
        /// </summary>
        /// <param name="text">The string for the label of the new tree node.</param>
        /// <param name="imageIndex">The image index of the unselected image in the parent TreeView's <see cref="http://msdn2.microsoft.com/en-us/library/system.windows.forms.treeview.imagelist.aspx">ImageList</see>.</param>
        /// <param name="selectedImageIndex">The image index of the selected image in the parent TreeView's <see cref="http://msdn2.microsoft.com/en-us/library/system.windows.forms.treeview.imagelist.aspx">ImageList</see>.</param>
        /// <param name="children">An array of child ThreeStateTreeNodes.</param>
        public ThreeStateTreeNode(string text, int imageIndex, int selectedImageIndex, ThreeStateTreeNode[] children) : base(text, imageIndex, selectedImageIndex, children) 
        {
            this.CommonConstructor(); 
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Performs common initialization to all constructors.
        /// </summary>
        private void CommonConstructor()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The current state of the checkbox.
        /// </summary>
        private CheckState mState = CheckState.Unchecked;
        [Category("Three State TreeView"),
        Description("The current state of the node's checkbox, Unchecked, Checked, or Indeterminate"),
        DefaultValue(CheckState.Unchecked),
        TypeConverter(typeof(CheckState)),
        Editor("Ascentium.Research.Windows.Components.CheckBoxState", typeof(CheckState))]
        public CheckState State
        {
            get { return this.mState; }
            set 
            {
                if (this.mState != value)
                {
                    this.mState = value;

                    // Ensure if checkboxes are used to make the checkbox checked or unchecked.
                    // When go to a fully drawn control, this will be managed in the drawing code.
                    // Setting the Checked property in code will cause the OnAfterCheck to be called
                    // and the action will be 'Unknown'; do not handle that case.
                    if ((this.TreeView != null) && (this.TreeView.CheckBoxes))
                        this.Checked = (this.mState == CheckState.Checked);

                    // TODO: Remove -> ONLY FOR DEMONSTRATION
                    // Change the display string of the node
                    // only for testing purposes to show the actual
                    // current state of the node.
                  //  this.Text = value.ToString();
                }
            }
        }

        /// <summary>
        /// Returns the 'combined' state for all siblings of a node.
        /// </summary>
        private CheckState SiblingsState
        {
            get
            {
                // If parent is null, cannot have any siblings or if the parent
                // has only one child (i.e. this node) then return the state of this 
                // instance as the state.
                if ((this.Parent == null) || (this.Parent.Nodes.Count == 1))
                    return this.State;

                // The parent has more than one child.  Walk through parent's child
                // nodes to determine the state of all this node's siblings,
                // including this node.
                CheckState state = 0;
                foreach (TreeNode node in this.Parent.Nodes)
                {
                    ThreeStateTreeNode child = node as ThreeStateTreeNode;
                    if (child != null)
                        state |= child.State;

                    // If the state is now indeterminate then know there
                    // is a combination of checked and unchecked nodes
                    // and no longer need to continue evaluating the rest
                    // of the sibling nodes.
                    if (state == CheckState.Indeterminate)
                       // state = CheckState.Indeterminate;
                        break;
                }

                return (state == 0) ? CheckState.Unchecked : state;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Manages state changes from one state to the next.
        /// </summary>
        /// <param name="fromState">The state upon which to base the state change.</param>
        public void Toggle(CheckState fromState)
        {
            switch (fromState)
            {
                case CheckState.Unchecked:
                    {
                        this.State = CheckState.Checked;
                        break;
                    }
                case CheckState.Checked:

                case CheckState.Indeterminate:
                default:
                    {
                        this.State = CheckState.Unchecked;
                        break;
                    }
            }

            this.UpdateStateOfRelatedNodes();
        }

        /// <summary>
        /// Manages state changes from one state to the next.
        /// </summary>
        public new void Toggle()
        {
            this.Toggle(this.State);
        }

        /// <summary>
        /// Manages updating related child and parent nodes of this instance.
        /// </summary>
        public void UpdateStateOfRelatedNodes()
        {
            ThreeStateTreeView tv = this.TreeView as ThreeStateTreeView;
            if ((tv != null) && tv.CheckBoxes && tv.UseThreeStateCheckBoxes)
            {
                tv.BeginUpdate();

                // If want to cascade checkbox state changes to child nodes of this node and
                // if the current state is not intermediate, update the state of child nodes.
                if (this.State != CheckState.Indeterminate)
                    this.UpdateChildNodeState();

                this.UpdateParentNodeState(true);

                tv.EndUpdate();
                
            }
        }

        /// <summary>
        /// Recursiveley update child node's state based on the state of this node.
        /// </summary>
        private void UpdateChildNodeState()
        {
            ThreeStateTreeNode child;
            foreach (TreeNode node in this.Nodes)
            {
                // It is possible node is not a ThreeStateTreeNode, so check first.
                if (node is ThreeStateTreeNode)
                {
                    child = node as ThreeStateTreeNode;
                    child.State = this.State;
                    child.Checked = (this.State != CheckState.Unchecked);
                    child.UpdateChildNodeState();
                }
            }
        }

        /// <summary>
        /// Recursiveley update parent node state based on the current state of this node.
        /// </summary>
        private void UpdateParentNodeState(bool isStartingPoint)
        {
            // If isStartingPoint is false, then know this is not the initial call
            // to the recursive method as we want to force on the first time
            // this is called to set the instance's parent node state based on
            // the state of all the siblings of this node, including the state
            // of this node.  So, if not the startpoint (!isStartingPoint) and
            // the state of this instance is indeterminate (CheckState.Indeterminate)
            // then know to set all subsequent parents to the indeterminate
            // state.  However, if not in an indeterminate state, then still need
            // to evaluate the state of all the siblings of this node, including the state
            // of this node before setting the state of the parent of this instance.
            var parent = this.Parent as ThreeStateTreeNode;
            if (parent != null)
            {
                CheckState state = CheckState.Unchecked;

                // Determine the new state
                if (!isStartingPoint && (this.State == CheckState.Indeterminate))
                {
                    state = CheckState.Indeterminate;
                }
                else
                {
                    state = this.SiblingsState;
                }

                // Update parent state if not the same.
                if (parent.State != state)
                {
                    parent.State = state;
                    parent.Checked = (state != CheckState.Unchecked);
                    parent.UpdateParentNodeState(false);
                }
            }
        }
        #endregion
    }
}
