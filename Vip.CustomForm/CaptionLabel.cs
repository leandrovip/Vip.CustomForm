using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Vip.CustomForm
{
    public class CaptionLabel
    {
        #region Private Properties

        private string labelText;

        private Point location;

        private Size labelSize;

        private Color labelBackColor;

        private Font labelFont;

        private Color foreColor;

        internal string name = string.Empty;

        private Form Owner;

        #endregion

        #region Public Properties

        [Browsable(true)]
        [Category("Design")]
        [Description("Gets/Sets the value for Label text.")]
        public string Text
        {
            get => labelText;
            set
            {
                labelText = value;
                if (Owner is FormBase) (Owner as FormBase)?.InvalidateFrame();
            }
        }

        [Description("Gets/Sets the value for label location.")]
        [Browsable(true)]
        [Category("Design")]
        public Point Location
        {
            get => location;
            set
            {
                location = value;
                if (Owner is FormBase) (Owner as FormBase)?.InvalidateFrame();
            }
        }

        [Browsable(true)]
        [Category("Design")]
        [Description("Gets/Sets the value for label size.")]
        public Size Size
        {
            get => labelSize;
            set
            {
                labelSize = value;
                if (Owner is FormBase) (Owner as FormBase)?.InvalidateFrame();
            }
        }

        [Category("Design")]
        [Description("Gets/Sets the value for label backcolor.")]
        [Browsable(true)]
        public Color BackColor
        {
            get => labelBackColor;
            set
            {
                labelBackColor = value;
                if (Owner is FormBase) (Owner as FormBase)?.InvalidateFrame();
            }
        }

        [Description("Gets/Sets the value for label font.")]
        [Category("Design")]
        [Browsable(true)]
        public Font Font
        {
            get => labelFont;
            set
            {
                labelFont = value;
                if (Owner is FormBase) (Owner as FormBase)?.InvalidateFrame();
            }
        }

        [DisplayName("(Name)")]
        [Browsable(true)]
        [Description("Gets or Sets the instance label name.")]
        [Category("Design")]
        public string Name
        {
            get => name;
            set => name = value;
        }

        [Category("Appearance")]
        [Browsable(true)]
        [Description("Gets or Sets the Forecolor of the CaptionLabel.")]
        public Color ForeColor
        {
            get => foreColor;
            set
            {
                foreColor = value;
                if (Owner is FormBase) (Owner as FormBase)?.InvalidateFrame();
            }
        }

        [Browsable(false)]
        public void SetOwner(Form value)
        {
            Owner = value;
        }

        #endregion

        #region Constructor

        public CaptionLabel()
        {
            labelText = "CaptionLabel";
            location = new Point(30, 4);
            labelSize = new Size(100, 24);
            labelBackColor = Color.Transparent;
            labelFont = SystemFonts.DefaultFont;
            foreColor = Color.Black;
        }

        #endregion
    }
}