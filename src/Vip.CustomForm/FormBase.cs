using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Vip.CustomForm
{
    public class FormBase : Form
    {
        private delegate IntPtr SendMessageDelegate(IntPtr hWnd, int msg, int wParam, int lParam);

        public ToolTip toolTip1;

        #region Private Class

        private class FrameLayoutInfo
        {
            private readonly FormBase m_form;

            private Rectangle m_rcText = Rectangle.Empty;

            public Rectangle m_rcIcon = Rectangle.Empty;

            public Rectangle m_rcMin = Rectangle.Empty;

            public Rectangle m_rcMax = Rectangle.Empty;

            public Rectangle m_rcClose = Rectangle.Empty;

            public Rectangle m_rcHelpButton = Rectangle.Empty;

            public Rectangle m_rcMdiIcon = Rectangle.Empty;

            public Rectangle m_rcMdiMin = Rectangle.Empty;

            public Rectangle m_rcMdiMax = Rectangle.Empty;

            public Rectangle m_rcMdiClose = Rectangle.Empty;

            public Rectangle m_rcMdiHelpButton = Rectangle.Empty;

            public Rectangle TextBox => m_rcText;

            public Rectangle IconBox => m_rcIcon;

            public Rectangle MinimizeBox => m_rcMin;

            public Rectangle MaximizeBox => m_rcMax;

            public Rectangle CloseBox => m_rcClose;

            public Rectangle HelpButton => m_rcHelpButton;

            public Rectangle MdiIconBox => m_rcMdiIcon;

            public Rectangle MdiMinimizeBox => m_rcMdiMin;

            public Rectangle MdiMaximizeBox => m_rcMdiMax;

            public Rectangle MdiCloseBox => m_rcMdiClose;

            public Rectangle MdiHelpButton => m_rcMdiHelpButton;

            public int TitleHeight { get; private set; }

            public int CaptionHeight { get; private set; }

            public int CaptionMinWidth { get; private set; }

            public int BorderWidth { get; private set; } = 6;

            private int SysBorderWidth
            {
                get
                {
                    CreateParams createParams = m_form.CreateParams;
                    NativeMethods.RECT rc = default;
                    if (AdjustWindowRectEx(ref rc, createParams.Style, false, createParams.ExStyle))
                    {
                        string[] array = Application.StartupPath.Split('\\');
                        if (array.Length > 0 && array[array.Length - 1] == "Office15") goto IL_009f;
                        if (array.Length > 0 && array[array.Length - 1] == "Office14") goto IL_009f;
                        if (array.Length > 0 && array[array.Length - 1] == "Office12") goto IL_009f;
                        return rc.Width / 2;
                    }

                    return 0;
                    IL_009f:
                    if (createParams.Style == 47120384) return rc.Width / 2;
                    return (rc.Width + 10) / 2;
                }
            }

            public FrameLayoutInfo(FormBase form)
            {
                m_form = form;
            }

            public void PerformLayout(int width, int height)
            {
                TitleHeight = 0;
                CaptionHeight = 0;
                CaptionMinWidth = 0;
                m_rcIcon = Rectangle.Empty;
                m_rcMin = Rectangle.Empty;
                m_rcMax = Rectangle.Empty;
                m_rcClose = Rectangle.Empty;
                m_rcHelpButton = Rectangle.Empty;
                m_rcMdiIcon = Rectangle.Empty;
                m_rcMdiMin = Rectangle.Empty;
                m_rcMdiMax = Rectangle.Empty;
                m_rcMdiClose = Rectangle.Empty;
                m_rcMdiHelpButton = Rectangle.Empty;
                m_rcText = Rectangle.Empty;
                bool flag = m_form.FormBorderStyle == FormBorderStyle.None;
                BorderWidth = !flag ? 6 : 0;
                Size size = SystemInformation.SmallIconSize;
                size = m_form.hasCaptionHeight && m_form.CaptionBarHeight < size.Height + 12 ? new Size(size.Width, 18) : size;
                if (m_form.EnableTouchMode) size = new Size(size.Width + 15, size.Height);
                bool isRightToLeft = m_form.IsRightToLeft;
                int num = m_form.captionBarHeight * 20 / 100;
                int num2 = m_form.captionBarHeight - num * 2;
                int num3 = (size.Width & -2) + 3;
                int num4 = m_form.hasCaptionHeight && m_form.CaptionBarHeight < size.Height + 12 ? num2 : size.Height + 2;
                int sysBorderWidth = SysBorderWidth;
                int num5 = sysBorderWidth + 2;
                int num7;
                int num8;
                if (!flag)
                {
                    Font captionFont = m_form.CaptionFont;
                    int num6 = Math.Max(captionFont.Height, num4);
                    if (!m_form.ShouldSerializeCaptionFont()) captionFont.Dispose();
                    if (!m_form.ControlBox && m_form.Text == string.Empty)
                    {
                        TitleHeight = BorderWidth;
                    }
                    else
                    {
                        int val = num5 + num6 + 2;
                        TitleHeight = m_form.hasCaptionHeight ? m_form.CaptionBarHeight : Math.Max(m_form.CaptionBarHeight, val);
                    }

                    CaptionHeight = TitleHeight + (m_form.WindowState == FormWindowState.Maximized && !m_form.IsMdiChild ? m_form.TitlePadding : 0);
                    int y = m_form.IsMinimized ? (height - num4) / 2 : sysBorderWidth + (TitleHeight - sysBorderWidth - 2 - num4) / 2;
                    num7 = num5;
                    num8 = width - num5;
                    if (m_form.ControlBox)
                    {
                        if (m_form.ShowIcon)
                        {
                            m_rcIcon.Size = size;
                            m_rcIcon.Y = y;
                            if (isRightToLeft)
                            {
                                num8 -= size.Width;
                                m_rcIcon.X = num8;
                            }
                            else
                            {
                                m_rcIcon.X = num7;
                                num7 = m_rcIcon.Right;
                            }

                            CaptionMinWidth += size.Width;
                        }

                        m_rcClose.Y = y;
                        m_rcClose.Width = num3;
                        m_rcClose.Height = num4;
                        if (isRightToLeft)
                        {
                            m_rcClose.X = num7;
                            num7 = m_rcClose.Right;
                        }
                        else
                        {
                            num8 -= num3;
                            if (m_form.EnableTouchMode)
                                m_rcClose.X = num8 - 5;
                            else
                                m_rcClose.X = num8;
                        }

                        CaptionMinWidth += num3;
                        if ((m_form.MaximizeBox || m_form.MinimizeBox) && !m_form.HideCaptionButtons)
                        {
                            m_rcMax = m_rcClose;
                            m_rcMin = m_rcClose;
                            if (isRightToLeft)
                            {
                                m_rcMax.X = num7;
                                m_rcMin.X = m_rcMax.Right;
                                num7 = m_rcMin.Right;
                            }
                            else
                            {
                                if (m_form.ShowMaximizeBox)
                                {
                                    m_rcMax.X = num8 - num3;
                                    num8 = m_rcMax.X;
                                }

                                if (m_form.ShowMinimizeBox)
                                {
                                    m_rcMin.X = m_rcMax.X - num3;
                                    num8 = m_rcMin.X;
                                }
                            }

                            CaptionMinWidth += 2 * num3;
                            goto IL_04e5;
                        }

                        if (m_form.HelpButton)
                        {
                            m_rcHelpButton = m_rcClose;
                            if (isRightToLeft)
                            {
                                m_rcHelpButton.X = num7;
                                num7 = m_rcHelpButton.Right;
                            }
                            else
                            {
                                num8 -= num3;
                                m_rcHelpButton.X = num8;
                            }

                            CaptionMinWidth += num3;
                        }
                    }

                    goto IL_04e5;
                }

                goto IL_0558;
                IL_04e5:
                m_rcText = new Rectangle(num7 + 4, 5, num8 - num7 - 8, TitleHeight - sysBorderWidth);
                if (m_form.IsRightToLeft) m_rcText.X = width - m_rcText.Right;
                m_rcText.Y += m_form.WindowState == FormWindowState.Maximized ? m_form.TitlePadding : 0;
                IL_0558:
                if (m_form.WindowState == FormWindowState.Maximized)
                {
                    m_rcClose.Y += m_form.TitlePadding;
                    m_rcMax.Y += m_form.TitlePadding;
                    m_rcMin.Y += m_form.TitlePadding;
                    m_rcHelpButton.Y += m_form.TitlePadding;
                }

                if (m_form.IsMdiContainer)
                {
                    Form activeMdiChild = m_form.ActiveMdiChild;
                    if (activeMdiChild != null && IsMaximized(activeMdiChild))
                    {
                        Control parent = activeMdiChild.Parent;
                        if (parent != null && NativeMethods.SendMessage(parent.Handle, 564, 0, 0) != IntPtr.Zero)
                        {
                            if (activeMdiChild.ControlBox)
                            {
                                if (activeMdiChild.ShowIcon)
                                {
                                    m_rcMdiIcon.X = isRightToLeft ? width - num5 - size.Width : num5;
                                    m_rcMdiIcon.Y = CaptionHeight + 1 - (m_form.WindowState == FormWindowState.Maximized ? m_form.TitlePadding : 0);
                                    m_rcMdiIcon.Size = size;
                                }

                                m_rcMdiClose.X = isRightToLeft ? num5 : width - num5 - num3;
                                m_rcMdiClose.Y = CaptionHeight;
                                m_rcMdiClose.Width = num3;
                                m_rcMdiClose.Height = num4;
                                if (activeMdiChild.MaximizeBox || activeMdiChild.MinimizeBox)
                                {
                                    m_rcMdiMax = m_rcMdiClose;
                                    m_rcMdiMin = m_rcMdiClose;
                                    if (isRightToLeft)
                                    {
                                        m_rcMdiMax.X = m_rcMdiClose.Right;
                                        m_rcMdiMin.X = m_rcMdiMax.Right;
                                    }
                                    else
                                    {
                                        m_rcMdiMax.X = m_rcMdiClose.X - num3;
                                        m_rcMdiMin.X = m_rcMdiMax.X - num3;
                                    }
                                }
                                else if (activeMdiChild.HelpButton)
                                {
                                    m_rcMdiHelpButton = m_rcMdiClose;
                                    if (isRightToLeft)
                                        m_rcMdiHelpButton.X = m_rcMdiClose.Right;
                                    else
                                        m_rcMdiHelpButton.X = m_rcMdiClose.X - num3;
                                }
                            }

                            CaptionHeight += num4 + 2;
                        }
                    }
                }
            }

            private static bool IsMaximized(Form f)
            {
                if (f.IsHandleCreated)
                {
                    int windowLong = NativeMethods.GetWindowLong(f.Handle, -16);
                    return (windowLong & 0x1000000) != 0;
                }

                return f.WindowState == FormWindowState.Maximized;
            }

            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            private static extern bool AdjustWindowRectEx(ref NativeMethods.RECT rc, int dwStyle, bool bMenu, int dwExStyle);
        }

        private class CaptionManager : IDisposable
        {
            private readonly FormBase m_form;

            private readonly int m_style;

            public CaptionManager(FormBase f, bool bHideCaption)
            {
                if (f != null && f.IsHandleCreated)
                {
                    IntPtr handle = f.Handle;
                    m_style = NativeMethods.GetWindowLong(handle, -16);
                    if (bHideCaption)
                    {
                        if ((m_style & 0xC00000) != 0 && !f.IsMdiChild)
                        {
                            NativeMethods.SetWindowLong(handle, -16, (IntPtr) (m_style & -12582913));
                            m_form = f;
                        }
                    }
                    else
                    {
                        int num = f.CreateParams.Style & 0xC00000;
                        if (num != 0 && (m_style & num) == 0 && (m_style & 0x40000) != 0)
                        {
                            NativeMethods.SetWindowLong(handle, -16, (IntPtr) (m_style | num));
                            m_form = f;
                        }
                    }
                }
            }

            void IDisposable.Dispose()
            {
                if (m_form != null && m_form.IsHandleCreated) NativeMethods.SetWindowLong(m_form.Handle, -16, (IntPtr) m_style);
            }
        }

        #endregion

        #region Private Properties

        private Rectangle IconBounds { get; set; }

        private int TitlePadding;

        private int imagePadding;

        private bool hasCaptionHeight;

        private Color metroColor = ColorTranslator.FromHtml("#119EDA");

        private Brush m_CaptionBarBrush;

        private Color captionBarColor = Color.White;

        private VerticalAlignment captionVerticalAlignment = VerticalAlignment.Center;

        private Color borderColor = ColorTranslator.FromHtml("#737373");

        private Color buttonColor = Color.DarkGray;

        private Color captionButtonColor = Color.DarkGray;

        private Color captionButtonHoverColor = Color.DarkGray;

        private int captionBarHeight = 26;

        private bool touchMode;

        private int borderThickness = 1;

        private bool showMaximizeBox;

        private bool showMinimizeBox;

        private Point mousePoint;

        private const int adjustvalueForDPI = 4;

        private bool innerBorderVisibility = true;

        private bool m_bActive;

        private bool m_bMouseIsTracked;

        private FrameLayoutInfo m_frameLayout;

        private int m_selectedButton = 4;

        private int m_pressedButton = 4;

        private int m_highlightedButton = 4;

        private static ImageList m_systemButtons;

        private static readonly int[] m_systemCommands;

        private Font m_captionFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);

        private HorizontalAlignment m_captionAlign;

        private HorizontalAlignment m_iconAlign;

        private LeftRightAlignment m_iconTextRelation;

        private Color captionForeColor = Color.Empty;

        private Point labelLocation;

        private Color Border;

        private IContainer components;

        private bool HideCaptionButtons { get; set; }

        private bool CustomizationApplied { get; set; }

        private int PressedButton
        {
            get => m_pressedButton;
            set
            {
                if (m_pressedButton != value)
                {
                    m_pressedButton = value;
                    if (value == 4) value = m_selectedButton;
                    m_highlightedButton = value;
                    InvalidateFrame();
                }
            }
        }

        private int HighlightedButton
        {
            get => m_highlightedButton;
            set
            {
                if (m_highlightedButton != value)
                {
                    m_highlightedButton = value;
                    InvalidateFrame();
                }
            }
        }

        private int MaximizeButton => WindowState != FormWindowState.Maximized ? 1 : 3;

        private int MinimizeButton => WindowState != FormWindowState.Minimized ? 2 : 3;

        private FrameLayoutInfo FrameLayout
        {
            get
            {
                if (m_frameLayout == null)
                {
                    m_frameLayout = new FrameLayoutInfo(this);
                    m_frameLayout.PerformLayout(Width, Height);
                }

                return m_frameLayout;
            }
        }

        private int CaptionHeight => FrameLayout.CaptionHeight;

        private int TitleHeight => FrameLayout.TitleHeight;

        private bool IsMinimized
        {
            get
            {
                if (IsHandleCreated)
                {
                    var windowLong = NativeMethods.GetWindowLong(Handle, -16);
                    return (windowLong & 0x20000000) != 0;
                }

                return WindowState == FormWindowState.Minimized;
            }
        }

        private static bool IsWindows7
        {
            get
            {
                if (Environment.OSVersion.Version.Major == 6) return Environment.OSVersion.Version.Minor == 1;
                return false;
            }
        }

        private int SelectedButton
        {
            get => m_selectedButton;
            set
            {
                if (m_selectedButton != value)
                {
                    m_selectedButton = value;
                    if (m_pressedButton != 4 && m_pressedButton != value) value = 4;
                    if (m_highlightedButton != value)
                    {
                        m_highlightedButton = value;
                        InvalidateFrame();
                    }
                }
            }
        }

        private bool IsMaximized
        {
            get
            {
                if (IsHandleCreated)
                {
                    int windowLong = NativeMethods.GetWindowLong(Handle, -16);
                    return (windowLong & 0x1000000) != 0;
                }

                return WindowState == FormWindowState.Maximized;
            }
        }

        private bool IsSizeable
        {
            get
            {
                bool result = false;
                if (WindowState == FormWindowState.Normal)
                {
                    int windowLong = NativeMethods.GetWindowLong(Handle, -16);
                    result = (windowLong & 0x40000) == 262144;
                }

                return result;
            }
        }

        private Rectangle DesktopRectangle
        {
            get
            {
                var screen = Screen.FromHandle(Handle);
                return new Rectangle(Screen.PrimaryScreen.WorkingArea.Location, screen.WorkingArea.Size);
            }
        }

        private Rectangle ParentClientRectangle
        {
            get
            {
                Control parent = Parent;
                if (parent != null && parent.IsHandleCreated)
                {
                    NativeMethods.RECT lpRect = default;
                    NativeMethods.GetClientRect(parent.Handle, ref lpRect);
                    return new Rectangle(0, 0, lpRect.Width, lpRect.Height);
                }

                return Rectangle.Empty;
            }
        }

        private bool IsVisible
        {
            get
            {
                bool result = false;
                if (IsHandleCreated)
                {
                    int windowLong = NativeMethods.GetWindowLong(Handle, -16);
                    result = (windowLong & 0x10000000) != 0;
                }

                return result;
            }
        }

        private IntPtr CaptionFontInternal
        {
            get
            {
                if (m_captionFont == null) return SystemCaptionFont;
                return m_captionFont.ToHfont();
            }
        }

        private IntPtr SystemCaptionFont
        {
            get
            {
                IntPtr result = IntPtr.Zero;
                NativeMethods.NONCLIENTMETRICS lpvParam = default;
                lpvParam.cbSize = Marshal.SizeOf(lpvParam);
                if (NativeMethods.SystemParametersInfo(41, 0, ref lpvParam, 0) != 0)
                {
                    lpvParam.lfCaptionFont.lfWeight = 400;
                    result = CreateFontIndirect(ref lpvParam.lfCaptionFont);
                }

                return result;
            }
        }

        private bool CloseBox => 0 == (CreateParams.ClassStyle & 0x200);

        #endregion

        #region Public Properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override bool AutoScroll
        {
            get => base.AutoScroll;
            set => base.AutoScroll = value;
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                UpdateFrame();
                Invalidate();
            }
        }

        [Description("Gets or Set the valur for BorderThickness.")]
        [Category("Appearance")]
        [Browsable(true)]
        public int BorderThickness
        {
            get => borderThickness;
            set
            {
                borderThickness = value;
                Invalidate();
                UpdateFrame();
            }
        }

        [Description("Gets or Sets the value for CaptionBarColor BorderColor.")]
        [Category("Appearance")]
        [Browsable(true)]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                UpdateFrame();
            }
        }

        [Category("Appearance")]
        [Description("Gets or Sets the value for Caption Bar Brush.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Brush CaptionBarBrush
        {
            get => m_CaptionBarBrush;
            set
            {
                m_CaptionBarBrush = value;
                UpdateFrame();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Gets or Sets the value for CaptionBarColor.")]
        public Color CaptionBarColor
        {
            get => captionBarColor;
            set
            {
                captionBarColor = value;
                UpdateFrame();
            }
        }

        [Browsable(true)]
        [Description("Gets/Sets the value for CaptionVerticalAlignment.")]
        [Category("Design")]
        public VerticalAlignment CaptionVerticalAlignment
        {
            get => captionVerticalAlignment;
            set
            {
                captionVerticalAlignment = value;
                UpdateFrame();
            }
        }

        [Browsable(true)]
        [Description("Gets/Sets the value for CaptionButtonColor.")]
        [Category("Appearance")]
        public Color CaptionButtonColor
        {
            get => captionButtonColor;
            set
            {
                captionButtonColor = value;
                buttonColor = value;
                UpdateFrame();
            }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [Description("Gets/Sets the value for CaptionButtonHoverColor.")]
        public Color CaptionButtonHoverColor
        {
            get => captionButtonHoverColor;
            set
            {
                captionButtonHoverColor = value;
                UpdateFrame();
            }
        }

        [Description("Gets or Sets value for CaptionBarHeight.")]
        [Browsable(true)]
        [Category("Appearance")]
        public int CaptionBarHeight
        {
            get => captionBarHeight;
            set
            {
                captionBarHeight = value;
                hasCaptionHeight = true;
                UpdateFrame();
            }
        }

        [Category("Appearance")]
        [Description("Gets or sets the font of the form's title.")]
        public Font CaptionFont
        {
            get
            {
                if (m_captionFont == null)
                {
                    IntPtr systemCaptionFont = SystemCaptionFont;
                    Font result;
                    if (systemCaptionFont != IntPtr.Zero)
                    {
                        result = Font.FromHfont(systemCaptionFont);
                        NativeMethods.DeleteObject(systemCaptionFont);
                    }
                    else
                    {
                        result = DefaultFont;
                    }

                    return result;
                }

                return m_captionFont;
            }
            set
            {
                if (!Equals(m_captionFont, value))
                {
                    m_captionFont = value;
                    UpdateFrame();
                }
            }
        }

        [DefaultValue(typeof(Color), "Empty")]
        [Description("Gets or sets the color for caption in titlebar.")]
        [Category("Appearance")]
        public Color CaptionForeColor
        {
            get
            {
                if (captionForeColor == Color.Empty) return Color.DimGray;
                return captionForeColor;
            }
            set
            {
                if (captionForeColor != value)
                {
                    captionForeColor = value;
                    UpdateFrame();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data")]
        [Description("Gets/Sets the label for form caption.")]
        public CaptionLabelCollection CaptionLabels { get; }

        [Description("Gets or sets the alignment of of the form's title.")]
        [Category("Appearance")]
        [DefaultValue(typeof(HorizontalAlignment), "Left")]
        public HorizontalAlignment CaptionAlign
        {
            get => m_captionAlign;
            set
            {
                if (m_captionAlign != value)
                {
                    m_captionAlign = value;
                    UpdateFrame();
                }
            }
        }

        [DefaultValue(false)]
        public bool EnableTouchMode
        {
            get => touchMode;
            set
            {
                if (value != touchMode)
                {
                    touchMode = value;
                    UpdateFrame();
                    if (touchMode)
                        ApplyScaleToControl(1.5f);
                    else
                        ApplyScaleToControl(1f);
                }
            }
        }

        [Category("Appearance")]
        [Description("Gets or sets the alignment of of the form's icon.")]
        [DefaultValue(typeof(HorizontalAlignment), "Left")]
        public HorizontalAlignment IconAlign
        {
            get => m_iconAlign;
            set
            {
                if (m_iconAlign != value)
                {
                    m_iconAlign = value;
                    UpdateFrame();
                }
            }
        }

        [DefaultValue(typeof(LeftRightAlignment), "Left")]
        [Description("Gets or sets the alignment between form's icon and form's text.")]
        [Category("Appearance")]
        public LeftRightAlignment IconTextRelation
        {
            get => m_iconTextRelation;
            set
            {
                if (m_iconTextRelation != value)
                {
                    m_iconTextRelation = value;
                    UpdateFrame();
                }
            }
        }

        [Description("Gets or Set Value to Drop Shadow to the form.")]
        [Category("Appearance")]
        [Browsable(true)]
        public bool DropShadow { get; set; }

        [Browsable(true)]
        [Description("Gets or Sets the value for CaptionBarColor BorderColor MetroColor.")]
        [Category("Appearance")]
        public Color MetroColor
        {
            get => metroColor;
            set
            {
                metroColor = value;
                UpdateFrame();
            }
        }

        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets / set the value to enable the MaximizeBox.")]
        [Browsable(true)]
        public bool ShowMaximizeBox
        {
            get => showMaximizeBox;
            set
            {
                if (showMaximizeBox != value)
                {
                    showMaximizeBox = value;
                    UpdateStyles();
                }
            }
        }

        [Browsable(true)]
        [Description("Gets / set the value to enable the MinimizeBox.")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowMinimizeBox
        {
            get => showMinimizeBox;
            set
            {
                if (showMinimizeBox != value)
                {
                    showMinimizeBox = value;
                    UpdateStyles();
                }
            }
        }

        [Description("Gets or Sets Mouse over color for CaptionButtons.")]
        [Browsable(true)]
        [Category("Appearance")]
        public bool ShowMouseOver { get; set; }

        [DefaultValue(true)]
        public bool InnerBorderVisibility
        {
            get => innerBorderVisibility;
            set
            {
                innerBorderVisibility = value;
                InvalidateFrame();
                UpdateRegion();
            }
        }

        #endregion

        #region Protected Properties

        protected bool IsActive
        {
            get
            {
                if (IsMdiChild)
                {
                    Form mdiParent = MdiParent;
                    if (mdiParent != null) return mdiParent.ActiveMdiChild == this;
                }

                return m_bActive;
            }
        }

        protected bool IsRightToLeft
        {
            get
            {
                bool result = false;
                if (IsHandleCreated)
                {
                    int windowLong = NativeMethods.GetWindowLong(Handle, -20);
                    result = (windowLong & 0x400000) != 0;
                }

                return result;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                if (DropShadow) createParams.ClassStyle |= 131072;
                return createParams;
            }
        }

        #endregion

        #region Protected Override Methods

        protected override void OnLoad(EventArgs e)
        {
            if (Tag != null && Tag.ToString() == "MultiScreen" && !MaximizeBox && !MinimizeBox)
                CustomizationApplied = true;
            else
                CustomizationApplied = false;
            base.OnLoad(e);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (specified == BoundsSpecified.None)
            {
                Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
                if (graphics.DpiX <= 96f)
                {
                    width -= 4;
                    height -= 4;
                }
                else if (graphics.DpiX >= 120f && graphics.DpiX < 144f)
                {
                    width -= 6;
                    height -= 6;
                }
                else if (graphics.DpiX >= 144f && graphics.DpiX < 168f)
                {
                    width -= 10;
                    height -= 10;
                }
                else if (graphics.DpiX >= 168f && graphics.DpiX < 192f)
                {
                    width -= 12;
                    height -= 12;
                }
                else if (graphics.DpiX >= 192f && graphics.DpiX < 216f)
                {
                    width -= 14;
                    height -= 14;
                }
                else if (graphics.DpiX >= 216f && graphics.DpiX < 240f)
                {
                    width -= 16;
                    height -= 16;
                }
                else if (graphics.DpiX >= 240f && graphics.DpiX < 264f)
                {
                    width -= 20;
                    height -= 20;
                }
                else if (graphics.DpiX >= 264f && graphics.DpiX < 288f)
                {
                    width -= 22;
                    height -= 22;
                }
                else if (graphics.DpiX >= 288f)
                {
                    width -= 24;
                    height -= 24;
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateRegion();
        }

        protected override void SetClientSizeCore(int x, int y)
        {
            int borderWidth = FrameLayout.BorderWidth;
            Size = new Size(x + 2 * borderWidth, y + CaptionHeight + borderWidth);
            UpdateBounds(Left, Top, x + 2 * borderWidth, y + CaptionHeight + borderWidth, x, y);
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            if (CustomizationApplied)
            {
                if (Tag != null && Tag.ToString() == "MultiScreen")
                {
                    HideCaptionButtons = true;
                    MinimizeBox = true;
                }
                else
                {
                    HideCaptionButtons = false;
                    MinimizeBox = false;
                }
            }

            base.OnLocationChanged(e);
        }

        protected override void OnStyleChanged(EventArgs e)
        {
            using (new CaptionManager(this, true)) base.OnStyleChanged(e);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 131:
                    if (!OnWmNcCalcSize(ref m)) break;
                    return;
                case 133:
                    if (!OnWmNcPaint(ref m)) break;
                    return;
                case 132:
                    if (!OnWmNcHitTest(ref m)) break;
                    return;
                case 134:
                    if (!OnWmNcActivate(ref m)) break;
                    return;
                case 160:
                    if (!OnWmNcMouseMove(ref m)) break;
                    return;
                case 674:
                    if (!OnWmNcMouseLeave(ref m)) break;
                    return;
                case 161:
                    if (!OnWmNcLButtonDown(ref m)) break;
                    return;
                case 512:
                    if (!OnWmMouseMove(ref m)) break;
                    return;
                case 162:
                {
                    NativeMethods.RECT lpRect = default;
                    NativeMethods.GetWindowRect((int) Handle, ref lpRect);
                    break;
                }
                case 514:
                    if (!OnWmLButtonUp(ref m)) break;
                    return;
                case 533:
                    if (!OnWmCaptureChanged(ref m)) break;
                    return;
                case 128:
                    if (!OnWmSetIcon(ref m)) break;
                    return;
                case 12:
                    if (!OnWmSetText(ref m)) break;
                    return;
                case 36:
                    if (!OnWmGetMinMaxInfo(ref m)) break;
                    return;
                case 70:
                    if (!OnWmWindowPosChanging(ref m)) break;
                    return;
                case 71:
                    if (!OnWmWindowPosChanged(ref m)) break;
                    return;
                case 32:
                    if (!OnWmSetCursor(ref m)) break;
                    return;
                case 274:
                    if (!OnWmSysCommand(ref m)) break;
                    return;
                case 123:
                    if (!OnWmCotextMenu(ref m)) break;
                    return;
            }

            base.WndProc(ref m);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Events

        public event PaintEventHandler CaptionBarPaint;

        #endregion

        #region Constructor

        static FormBase()
        {
            Bitmap value = new Bitmap(typeof(FormBase).Assembly.GetManifestResourceStream("Vip.CustomForm.Images.SystemButtons.bmp"));
            m_systemButtons = new ImageList();
            m_systemButtons.ImageSize = new Size(9, 9);
            m_systemButtons.Images.AddStrip(value);
            m_systemButtons.TransparentColor = Color.Magenta;
            m_systemCommands = new int[4]
            {
                61536,
                61488,
                61472,
                61728
            };
        }

        public FormBase()
        {
            toolTip1 = new ToolTip
            {
                AutomaticDelay = 500,
                ShowAlways = true,
                AutoPopDelay = 10000,
                InitialDelay = 100,
                ReshowDelay = 100
            };

            CaptionLabels = new CaptionLabelCollection(this);
            BackColor = Color.White;
            CaptionAlign = HorizontalAlignment.Center;
            IconAlign = HorizontalAlignment.Left;
            IconTextRelation = LeftRightAlignment.Left;
            CaptionForeColor = ColorTranslator.FromHtml("#343434");
            showMinimizeBox = true;
            showMaximizeBox = true;
            base.AutoScroll = false;
            UpdateTitlePadding();
        }

        #endregion

        #region Public Methods

        public void ResetShowMaximizeBox()
        {
            ShowMaximizeBox = true;
        }

        public void ApplyScaleToControl(float sf)
        {
            SuspendLayout();
            foreach (Control control in Controls)
            {
                PropertyInfo property = control.GetType().GetProperty("EnableTouchMode");
                if (property != null) property.SetValue(control, touchMode, null);
                Touch(control);
            }

            ResumeLayout();
            Invalidate();
        }

        #endregion

        #region Private Methods

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(609, 422);
            Name = "FormBase";
            Text = "FormBase";
            ResumeLayout(false);
        }

        private void UpdateTitlePadding()
        {
            TitlePadding = 0;
        }

        private bool GetCloseBox(Control control)
        {
            bool result = true;
            if (control.IsHandleCreated)
            {
                int num = (int) GetClassLongPtr(control.Handle, -26);
                result = 0 == (num & 0x200);
            }

            return result;
        }

        private void Touch(Control ctr)
        {
            foreach (Control control in ctr.Controls)
            {
                PropertyInfo property = control.GetType().GetProperty("EnableTouchMode");
                if (property != null)
                {
                    if (!(control is Button))
                        property.SetValue(control, touchMode, null);
                    else if (control.Parent == null || control.Parent is Form || control.Parent is UserControl || control.Parent is Panel) property.SetValue(control, touchMode, null);
                }

                Touch(control);
            }
        }

        private bool OnWmNcCalcSize(ref Message m)
        {
            NativeMethods.RECT structure = (NativeMethods.RECT) m.GetLParam(typeof(NativeMethods.RECT));
            using (new CaptionManager(this, true)) base.WndProc(ref m);
            FrameLayoutInfo frameLayout = FrameLayout;
            int borderWidth = frameLayout.BorderWidth;
            if (!DesignMode && !InnerBorderVisibility) borderWidth = BorderThickness;
            if (MaximizedBounds.X != 0 || MaximizedBounds.Y != 0 || MaximizedBounds.Width != 0 || MaximizedBounds.Height != 0)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    NativeMethods.RECT structure2 = new NativeMethods.RECT(MaximizedBounds);
                    frameLayout.PerformLayout(structure2.Width, structure2.Height);
                    structure2.top += frameLayout.CaptionHeight - 8;
                    structure2.left += BorderThickness - 8;
                    structure2.right -= borderWidth;
                    structure2.bottom -= borderWidth;
                    Marshal.StructureToPtr(structure2, m.LParam, true);
                    UpdateBounds();
                }
                else
                {
                    frameLayout.PerformLayout(structure.Width, structure.Height);
                    structure.top += frameLayout.CaptionHeight;
                    structure.left += borderWidth;
                    structure.right -= borderWidth;
                    structure.bottom -= borderWidth;
                    Marshal.StructureToPtr(structure, m.LParam, true);
                }
            }
            else
            {
                frameLayout.PerformLayout(structure.Width, structure.Height);
                structure.top += frameLayout.CaptionHeight;
                if (WindowState == FormWindowState.Maximized && FormBorderStyle != 0)
                {
                    imagePadding = IsWindows7 && (FormBorderStyle == FormBorderStyle.SizableToolWindow || FormBorderStyle == FormBorderStyle.FixedToolWindow) ? -6 : IsMdiChild ? -6 : -8;
                    structure.left -= imagePadding;
                    structure.right += imagePadding;
                    structure.bottom += imagePadding;
                }
                else if (InnerBorderVisibility || DesignMode)
                {
                    imagePadding = 0;
                    structure.left += borderWidth;
                    structure.right -= borderWidth;
                    structure.bottom -= borderWidth;
                }
                else
                {
                    imagePadding = 0;
                    structure.left += borderWidth - borderWidth / 3;
                    structure.right -= borderWidth - borderWidth / 3;
                    structure.bottom -= borderWidth - borderWidth / 3;
                }

                Marshal.StructureToPtr(structure, m.LParam, true);
            }

            m.Result = IntPtr.Zero;
            return true;
        }

        private bool OnWmNcPaint(ref Message m)
        {
            IntPtr windowDC = NativeMethods.GetWindowDC(Handle);
            if (windowDC != IntPtr.Zero)
            {
                NativeMethods.RECT lpRect = default;
                NativeMethods.GetWindowRect((int) Handle, ref lpRect);
                NativeMethods.GetActiveWindow();
                if (m.WParam.ToInt64() != 1)
                {
                    int regionData = NativeMethods.GetRegionData(m.WParam, 0, IntPtr.Zero);
                    if (regionData > 0)
                    {
                        IntPtr intPtr = Marshal.AllocHGlobal(regionData);
                        if (NativeMethods.GetRegionData(m.WParam, regionData, intPtr) > 0)
                        {
                            Matrix matrix = new Matrix();
                            matrix.Translate(-lpRect.left, -lpRect.top, MatrixOrder.Append);
                            if (IsRightToLeft)
                            {
                                matrix.Scale(-1f, 1f, MatrixOrder.Append);
                                matrix.Translate(lpRect.Width, 0f, MatrixOrder.Append);
                            }

                            NativeMethods.XFORM lpXform = default;
                            lpXform.eM11 = matrix.Elements[0];
                            lpXform.eM12 = matrix.Elements[1];
                            lpXform.eM21 = matrix.Elements[2];
                            lpXform.eM22 = matrix.Elements[3];
                            lpXform.eDx = matrix.Elements[4];
                            lpXform.eDy = matrix.Elements[5];
                            IntPtr intPtr2 = NativeMethods.ExtCreateRegion(ref lpXform, regionData, intPtr);
                            if (intPtr2 != IntPtr.Zero)
                            {
                                NativeMethods.SelectClipRgn(windowDC, intPtr2);
                                NativeMethods.DeleteObject(intPtr2);
                            }
                        }

                        Marshal.FreeHGlobal(intPtr);
                    }
                }

                IntPtr intPtr3 = NativeMethods.CreateCompatibleDC(windowDC);
                if (intPtr3 != IntPtr.Zero)
                {
                    IntPtr intPtr4 = NativeMethods.CreateCompatibleBitmap(windowDC, lpRect.Width, lpRect.Height);
                    if (intPtr4 != IntPtr.Zero)
                    {
                        NativeMethods.SelectObject(intPtr3, intPtr4);
                        using (Graphics g = Graphics.FromHdc(intPtr3))
                        {
                            int borderWidth = FrameLayout.BorderWidth;
                            DrawFrame(g, new Rectangle(0, 0, lpRect.Width, lpRect.Height));
                            if (!DesignMode && !InnerBorderVisibility) borderWidth = BorderThickness;
                            int num = lpRect.Width - borderWidth;
                            if (num - borderWidth > 0)
                            {
                                int captionHeight = CaptionHeight;
                                int num2 = lpRect.Height - borderWidth;
                                if (num2 - captionHeight > 0) NativeMethods.ExcludeClipRect(windowDC, borderWidth, captionHeight, num, num2);
                            }

                            NativeMethods.BitBlt(windowDC, 0, 0, lpRect.Width, lpRect.Height, intPtr3, 0, 0, 13369376);
                        }

                        NativeMethods.DeleteObject(intPtr4);
                    }

                    NativeMethods.DeleteDC(intPtr3);
                }

                NativeMethods.ReleaseDC(Handle, windowDC);
            }

            m.Result = IntPtr.Zero;
            return true;
        }

        private bool OnWmNcHitTest(ref Message m)
        {
            bool result = false;
            int x = NativeMethods.LOWORD(m.LParam);
            int y = NativeMethods.HIWORD(m.LParam);
            mousePoint = new Point(x, y);
            int hitTest = GetHitTest(x, y);
            if (hitTest != -2)
            {
                m.Result = (IntPtr) hitTest;
                result = true;
            }

            return result;
        }

        private bool OnWmNcActivate(ref Message m)
        {
            bool result = false;
            m_bActive = m.WParam != IntPtr.Zero;
            if (!m_bActive) m_highlightedButton = 4;
            if (IsVisible)
            {
                if (!IsMdiContainer) NativeMethods.LockWindowUpdate(Handle);
                base.WndProc(ref m);
                NativeMethods.LockWindowUpdate(IntPtr.Zero);
                Message m2 = default;
                m2.Msg = 133;
                m2.HWnd = m.HWnd;
                m2.WParam = (IntPtr) 1;
                m2.LParam = (IntPtr) 0;
                OnWmNcPaint(ref m2);
                result = true;
            }

            return result;
        }

        private bool OnWmNcMouseMove(ref Message m)
        {
            SelectedButton = GetButtonId(m.LParam);
            NativeMethods.RECT rc = default;
            NativeMethods.GetWindowRect((int) Handle, ref rc);

            if (!m_bMouseIsTracked)
            {
                NativeMethods.TRACKMOUSEEVENT lpEventTrack = default;
                lpEventTrack.cbSize = Marshal.SizeOf(lpEventTrack);
                lpEventTrack.hwndTrack = Handle;
                lpEventTrack.dwFlags = 18;
                NativeMethods.TrackMouseEvent(ref lpEventTrack);
                m_bMouseIsTracked = true;
            }

            m.Result = IntPtr.Zero;
            return true;
        }

        private bool OnWmNcMouseLeave(ref Message m)
        {
            SelectedButton = 4;
            m_bMouseIsTracked = false;
            return false;
        }

        private bool OnWmNcLButtonDown(ref Message m)
        {
            NativeMethods.RECT rc = default;
            NativeMethods.GetWindowRect((int) Handle, ref rc);

            var buttonId = GetButtonId(m.LParam);
            var result = false;
            if (buttonId != 4)
            {
                if (IsButtonEnabled(buttonId))
                {
                    PressedButton = buttonId;
                    Capture = true;
                }

                m.Result = IntPtr.Zero;
                result = true;
            }
            else
            {
                using (new CaptionManager(this, true)) result = false;
                if (IsMdiChild) UpdateStyle();
            }

            return result;
        }

        private bool OnWmMouseMove(ref Message m)
        {
            if (Capture)
            {
                Point p = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));
                SelectedButton = GetButtonId(PointToScreen(p));
            }

            return false;
        }

        private bool OnWmLButtonUp(ref Message m)
        {
            if (PressedButton != 4)
            {
                Point p = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));
                int controlButtonId = GetControlButtonId(PointToScreen(p));
                if (controlButtonId == PressedButton)
                {
                    Form form = controlButtonId < 4 || controlButtonId == 20 ? this : ActiveMdiChild;
                    if (form != null && form.IsHandleCreated) form.BeginInvoke(new SendMessageDelegate(NativeMethods.SendMessage), form.Handle, 274, GetButtonCommand(controlButtonId), 0);
                }
            }

            return false;
        }

        private bool OnWmCaptureChanged(ref Message m)
        {
            PressedButton = 4;
            return false;
        }

        private bool OnWmSetIcon(ref Message m)
        {
            BaseWndProc(ref m);
            FrameLayout.PerformLayout(Width, Height);
            InvalidateFrame();
            return true;
        }

        private bool OnWmSetText(ref Message m)
        {
            BaseWndProc(ref m);
            InvalidateFrame();
            return true;
        }

        private bool OnWmGetMinMaxInfo(ref Message m)
        {
            NativeMethods.MINMAXINFO structure = (NativeMethods.MINMAXINFO) m.GetLParam(typeof(NativeMethods.MINMAXINFO));
            Rectangle maxRectangle = GetMaxRectangle();
            int width = MaximumSize.Width;
            int height = MaximumSize.Height;
            int width2 = MinimumSize.Width;
            int height2 = MinimumSize.Height;
            if (FormBorderStyle != FormBorderStyle.SizableToolWindow && FormBorderStyle != FormBorderStyle.FixedToolWindow) goto IL_0079;
            if (IsWindows7) goto IL_0079;
            IL_00c5:
            if (width > 0)
            {
                if (structure.ptMaxSize.X > width) structure.ptMaxSize.X = width;
                if (structure.ptMaxTrackSize.X > width) structure.ptMaxTrackSize.X = width;
            }

            if (height > 0)
            {
                if (structure.ptMaxSize.Y > height) structure.ptMaxSize.Y = height;
                if (structure.ptMaxTrackSize.Y > height) structure.ptMaxTrackSize.Y = height;
            }

            FrameLayoutInfo frameLayout = FrameLayout;
            int borderWidth = frameLayout.BorderWidth;
            if (!InnerBorderVisibility) borderWidth = BorderThickness;
            int num = frameLayout.CaptionMinWidth + borderWidth * 2;
            int num2 = frameLayout.CaptionHeight + borderWidth;
            structure.ptMinTrackSize.X = width2 > 0 && width2 > num ? width2 : num;
            structure.ptMinTrackSize.Y = height2 > 0 && height2 > num2 ? height2 : num2;
            Marshal.StructureToPtr(structure, m.LParam, false);
            m.Result = IntPtr.Zero;
            return true;
            IL_0079:
            structure.ptMaxPosition.X = maxRectangle.X;
            structure.ptMaxPosition.Y = maxRectangle.Y;
            structure.ptMaxSize.X = maxRectangle.Width;
            structure.ptMaxSize.Y = maxRectangle.Height;
            goto IL_00c5;
        }

        private bool OnWmWindowPosChanging(ref Message m)
        {
            base.WndProc(ref m);
            if (!TopLevel && IsMaximized)
            {
                NativeMethods.WINDOWPOS structure = (NativeMethods.WINDOWPOS) m.GetLParam(typeof(NativeMethods.WINDOWPOS));
                if ((structure.flags & 3) != 3)
                {
                    Rectangle maxRectangle = GetMaxRectangle();
                    structure.x = maxRectangle.X;
                    structure.y = maxRectangle.Y;
                    structure.cx = maxRectangle.Width;
                    structure.cy = maxRectangle.Height;
                    structure.flags &= -4;
                    Marshal.StructureToPtr(structure, m.LParam, false);
                }
            }

            return true;
        }

        private bool OnWmWindowPosChanged(ref Message m)
        {
            bool result = false;
            NativeMethods.WINDOWPOS wINDOWPOS = (NativeMethods.WINDOWPOS) m.GetLParam(typeof(NativeMethods.WINDOWPOS));
            if ((wINDOWPOS.flags & 1) == 0)
            {
                SelectedButton = 4;
                UpdateRegion();
                base.WndProc(ref m);
                Invalidate();
                UpdateRegion();
                result = true;
            }

            return result;
        }

        private bool OnWmSetCursor(ref Message m)
        {
            Cursor cursor;
            switch (NativeMethods.LOWORD(m.LParam))
            {
                case 12:
                case 15:
                    cursor = Cursors.SizeNS;
                    break;
                case 10:
                case 11:
                    cursor = Cursors.SizeWE;
                    break;
                case 13:
                case 17:
                    cursor = Cursors.SizeNWSE;
                    break;
                case 14:
                case 16:
                    cursor = Cursors.SizeNESW;
                    break;
                default:
                    return false;
            }

            NativeMethods.SetCursor(cursor.Handle);
            m.Result = (IntPtr) 1;
            return true;
        }

        private bool OnWmSysCommand(ref Message m)
        {
            int num = (int) m.WParam & 0xFFF0;
            if (num != 61584 && num != 61696 && num != 61824) return false;
            BaseWndProc(ref m);
            return true;
        }

        private bool OnWmCotextMenu(ref Message m)
        {
            if (m.WParam == m.HWnd)
            {
                Point p = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));
                if (PointToClient(p).Y < 0)
                {
                    IntPtr systemMenu = NativeMethods.GetSystemMenu(Handle, false);
                    if (systemMenu != IntPtr.Zero)
                    {
                        int num = TrackPopupMenu(systemMenu, 256u, p.X, p.Y, 0, m.HWnd, IntPtr.Zero);
                        if (num != 0) NativeMethods.SendMessage(m.HWnd, 274, num, m.LParam);
                    }

                    return true;
                }
            }

            return false;
        }

        private void BaseWndProc(ref Message m)
        {
            using (new CaptionManager(this, true)) base.WndProc(ref m);
        }

        private void UpdateStyle()
        {
            IntPtr handle = Handle;
            int windowLong = NativeMethods.GetWindowLong(handle, -16);
            if ((windowLong & 0xC00000) != 0) NativeMethods.SetWindowLong(handle, -16, (IntPtr) (windowLong & -12582913));
        }

        private void UpdateRegion()
        {
            if (IsHandleCreated)
            {
                IntPtr handle = Handle;
                if (FormBorderStyle != 0)
                {
                    NativeMethods.RECT lpRect = default;
                    NativeMethods.GetWindowRect((int) handle, ref lpRect);
                    if (!IsMaximized || IsMaximized && IsMdiChild)
                    {
                        IntPtr intPtr = NativeMethods.CreateRectRgn(0, 0, lpRect.Width + 1, lpRect.Height + 1);
                        if (intPtr != IntPtr.Zero)
                        {
                            IntPtr intPtr2 = NativeMethods.CreateRectRgn(0, 0, lpRect.Width + 1, lpRect.Height + 6 + 1);
                            if (intPtr2 != IntPtr.Zero)
                            {
                                if (NativeMethods.CombineRgn(intPtr, intPtr, intPtr2, 1) != 0)
                                {
                                    NativeMethods.SetWindowRgn(handle, intPtr, true);
                                    intPtr = IntPtr.Zero;
                                }

                                NativeMethods.DeleteObject(intPtr2);
                            }

                            NativeMethods.DeleteObject(intPtr);
                        }
                    }
                    else
                    {
                        Screen screen = Screen.FromHandle(handle);
                        if (screen != null)
                        {
                            Rectangle a = new Rectangle(lpRect.left, lpRect.top, lpRect.Width, lpRect.Height);
                            Rectangle rectangle = Rectangle.Intersect(a, screen.WorkingArea);
                            rectangle.Offset(-a.X, -a.Y);
                            IntPtr intPtr3 = NativeMethods.CreateRectRgn(rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom);
                            if (MaximizedBounds.X != 0 || MaximizedBounds.Y != 0 || MaximizedBounds.Width != 0 || MaximizedBounds.Height != 0)
                            {
                                MaximizedBounds.Offset(-a.X, -a.Y);
                                intPtr3 = NativeMethods.CreateRectRgn(MaximizedBounds.X, MaximizedBounds.Y, MaximizedBounds.Right, MaximizedBounds.Bottom);
                                Bounds = MaximizedBounds;
                            }

                            if (intPtr3 != IntPtr.Zero) NativeMethods.SetWindowRgn(handle, intPtr3, true);
                        }
                    }
                }
                else
                {
                    NativeMethods.SetWindowRgn(handle, IntPtr.Zero, true);
                }
            }
        }

        private void UpdateFrame()
        {
            m_frameLayout = null;
            if (IsHandleCreated)
            {
                if (MdiParent != null && WindowState == FormWindowState.Maximized)
                    NativeMethods.SetWindowPos(MdiParent.Handle, IntPtr.Zero, 0, 0, 0, 0, 55);
                else
                    NativeMethods.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, 55);
            }
        }

        internal void InvalidateFrame()
        {
            if (IsHandleCreated) NativeMethods.RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, 1025u);
        }

        private int GetHitTest(int x, int y)
        {
            NativeMethods.RECT rc = default;
            NativeMethods.GetWindowRect((int) Handle, ref rc);
            if (NativeMethods.PtInRect(ref rc, new NativeMethods.POINT(x, y)))
            {
                int num = x - rc.left;
                int num2 = y - rc.top;
                int num3 = rc.Width - num;
                int num4 = rc.Height - num2;
                FrameLayoutInfo frameLayout = FrameLayout;
                int captionHeight = frameLayout.CaptionHeight;
                int num5 = 6;
                int num6 = WindowState == FormWindowState.Maximized ? TitlePadding : 0;
                if (!InnerBorderVisibility) num5 = BorderThickness;
                if (num >= num5 && num3 >= num5 && num2 >= captionHeight && num4 >= num5) goto IL_02b4;
                if (IsSizeable)
                {
                    if (num < num5)
                    {
                        if (num2 < captionHeight) return 13;
                        if (num4 < num5) return 16;
                        return 10;
                    }

                    if (num3 < num5)
                    {
                        if (num2 < captionHeight) return 14;
                        if (num4 < num5) return 17;
                        return 11;
                    }

                    if (num2 < num5) return 12;
                    if (num4 < num5) return 15;
                }

                if (num2 < TitleHeight)
                {
                    if (frameLayout.IconBox.Contains(num, num2)) return 3;
                    if (frameLayout.CloseBox.Contains(num, num2)) return 20;
                    if (frameLayout.MaximizeBox.Contains(num, num2)) return 9;
                    if (frameLayout.MinimizeBox.Contains(num, num2)) return 8;
                    if (frameLayout.HelpButton.Contains(num, num2)) return 21;
                    foreach (CaptionLabel captionLabel in CaptionLabels)
                        if (new Rectangle(captionLabel.Location.X + num6, captionLabel.Location.Y + num6, captionLabel.Size.Width, captionLabel.Size.Height).Contains(num, num2))
                            return 111;
                    return 2;
                }

                return 5;
            }

            IL_02b4:
            return -2;
        }

        private int GetButtonId(IntPtr points)
        {
            int x = NativeMethods.LOWORD(points);
            int y = NativeMethods.HIWORD(points);
            return GetButtonId(new Point(x, y));
        }

        private int GetControlButtonId(Point pt)
        {
            NativeMethods.RECT lpRect = default;
            NativeMethods.GetWindowRect((int) Handle, ref lpRect);
            int x = pt.X - lpRect.left;
            int y = pt.Y - lpRect.top;
            FrameLayoutInfo frameLayout = FrameLayout;
            if (frameLayout.CloseBox.Contains(x, y)) return 0;
            if (frameLayout.MaximizeBox.Contains(x, y) && ShowMaximizeBox) return MaximizeButton;
            if (frameLayout.MinimizeBox.Contains(x, y) && ShowMinimizeBox) return MinimizeButton;
            if (frameLayout.HelpButton.Contains(x, y)) return 20;
            if (frameLayout.MdiMaximizeBox.Contains(x, y)) return 19;
            if (frameLayout.MdiMinimizeBox.Contains(x, y)) return 18;
            if (frameLayout.MdiCloseBox.Contains(x, y)) return 16;
            if (frameLayout.MdiHelpButton.Contains(x, y)) return 21;
            return 4;
        }

        private int GetButtonId(Point pt)
        {
            NativeMethods.RECT lpRect = default;
            NativeMethods.GetWindowRect((int) Handle, ref lpRect);
            int x = pt.X - lpRect.left;
            int y = pt.Y - lpRect.top;
            FrameLayoutInfo frameLayout = FrameLayout;
            if (frameLayout.CloseBox.Contains(x, y)) return 0;
            if (frameLayout.MaximizeBox.Contains(x, y)) return MaximizeButton;
            if (frameLayout.MinimizeBox.Contains(x, y)) return MinimizeButton;
            if (frameLayout.HelpButton.Contains(x, y)) return 20;
            if (frameLayout.MdiMaximizeBox.Contains(x, y)) return 19;
            if (frameLayout.MdiMinimizeBox.Contains(x, y)) return 18;
            if (frameLayout.MdiCloseBox.Contains(x, y)) return 16;
            if (frameLayout.MdiHelpButton.Contains(x, y)) return 21;
            return 4;
        }

        private bool IsButtonEnabled(int button)
        {
            bool result = true;
            switch (button)
            {
                case 1:
                    result = MaximizeBox;
                    break;
                case 2:
                    result = MinimizeBox;
                    break;
                case 0:
                    result = CloseBox;
                    break;
                case 18:
                    if (IsMdiContainer)
                    {
                        Form activeMdiChild = ActiveMdiChild;
                        if (activeMdiChild != null) result = activeMdiChild.MinimizeBox;
                    }

                    break;
            }

            return result;
        }

        private Rectangle GetMaxRectangle()
        {
            Rectangle result = TopLevel ? DesktopRectangle : ParentClientRectangle;
            int borderWidth = FrameLayout.BorderWidth;
            if (IsMdiChild)
            {
                result.X -= borderWidth;
                result.Y -= CaptionHeight;
                result.Width += 2 * borderWidth;
                result.Height += CaptionHeight + borderWidth;
            }
            else
            {
                result = new Rectangle(-borderWidth, -borderWidth, result.Width + 2 * borderWidth, result.Height + 2 * borderWidth);
            }

            return result;
        }

        private Image GetButtonImage(int buttonID)
        {
            Image result = null;
            switch (buttonID)
            {
                default:
                {
                    int num = 1;
                    try
                    {
                        result = m_systemButtons.Images[num * 4 + (buttonID & 0xF)];
                        return result;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        UpdateSystemButtonsImages();
                        return m_systemButtons.Images[num * 4 + (buttonID & 0xF)];
                    }
                    catch
                    {
                        return result;
                    }
                }
            }
        }

        private void UpdateSystemButtonsImages()
        {
            m_systemButtons.Images.Clear();
            m_systemButtons = null;
            var value = new Bitmap(typeof(FormBase).Assembly.GetManifestResourceStream("Vip.CustomForm.Images.SystemButtons.bmp"));
            m_systemButtons = new ImageList();
            m_systemButtons.ImageSize = new Size(9, 9);
            m_systemButtons.Images.AddStrip(value);
            m_systemButtons.TransparentColor = Color.Magenta;
        }

        private int GetButtonCommand(int buttonID)
        {
            switch (buttonID)
            {
                default:
                    return m_systemCommands[buttonID & 3];
            }
        }

        private bool ShouldSerializeCaptionFont()
        {
            return m_captionFont != null;
        }

        #endregion

        #region Draw Methods

        private void DrawFrame(Graphics g, Rectangle rc)
        {
            DrawFrameBackground(g, rc);
            DrawFrameCaption(g, rc);
            DrawLabel(g, rc);
            if (FormBorderStyle != 0) DrawFrameBorders(g, rc);
        }

        private void DrawLabel(Graphics g, Rectangle rc)
        {
            foreach (CaptionLabel captionLabel in CaptionLabels)
            {
                switch (RightToLeft)
                {
                    case RightToLeft.No:
                        labelLocation = new Point(captionLabel.Location.X, captionLabel.Location.Y);
                        break;
                    case RightToLeft.Yes:
                        labelLocation = new Point(Width - (captionLabel.Location.X + captionLabel.Size.Width), captionLabel.Location.Y);
                        break;
                }

                if (WindowState == FormWindowState.Maximized)
                    g.FillRectangle(new SolidBrush(captionLabel.BackColor),
                        new Rectangle(labelLocation.X + TitlePadding, labelLocation.Y + TitlePadding, captionLabel.Size.Width, captionLabel.Size.Height));
                else
                    g.FillRectangle(new SolidBrush(captionLabel.BackColor), new Rectangle(labelLocation.X, labelLocation.Y, captionLabel.Size.Width, captionLabel.Size.Height));
                if (captionLabel.Text != string.Empty)
                    using (StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic))
                    {
                        stringFormat.Trimming = StringTrimming.EllipsisWord;
                        Rectangle r = Rectangle.Empty;
                        r = WindowState != FormWindowState.Maximized
                            ? new Rectangle(labelLocation.X, captionLabel.Location.Y + captionLabel.Size.Height / 2 - TextRenderer.MeasureText(captionLabel.Text, captionLabel.Font).Height / 2,
                                captionLabel.Size.Width, captionLabel.Size.Height)
                            : new Rectangle(labelLocation.X + TitlePadding,
                                captionLabel.Location.Y + TitlePadding + captionLabel.Size.Height / 2 - TextRenderer.MeasureText(captionLabel.Text, captionLabel.Font).Height / 2,
                                captionLabel.Size.Width, captionLabel.Size.Height);
                        g.DrawString(captionLabel.Text, captionLabel.Font, new SolidBrush(captionLabel.ForeColor), r, stringFormat);
                    }
            }
        }

        private void DrawFrameBackground(Graphics g, Rectangle rc)
        {
            int x = rc.Left - 1;
            int width = rc.Width + 1;
            int num = TitleHeight + (WindowState == FormWindowState.Maximized ? TitlePadding : 0);
            Rectangle empty = Rectangle.Empty;
            if (num > 0)
            {
                if (CaptionBarPaint != null)
                {
                    empty = new Rectangle(x, rc.Top, width, num);
                    CaptionBarPaint(this, new PaintEventArgs(g, empty));
                }
                else if (CaptionBarBrush != null)
                {
                    g.FillRectangle(rect: new Rectangle(x, rc.Top, width, num), brush: CaptionBarBrush);
                }
                else
                {
                    g.FillRectangle(rect: new Rectangle(x, rc.Top, width, num), brush: new SolidBrush(CaptionBarColor));
                }
            }

            if (IsMdiContainer)
            {
                FormBase FormBase = ActiveMdiChild as FormBase;
                Color color = FormBase?.CaptionBarColor ?? BackColor;
                int num2 = FormBase?.CaptionBarHeight ?? num;
                Brush brush = new SolidBrush(color);
                g.FillRectangle(brush, new Rectangle(x, rc.Top + num, width, num2));
                brush.Dispose();
                num += num2;
            }

            Brush brush2 = new SolidBrush(BackColor);
            g.FillRectangle(brush2, new Rectangle(x, rc.Top + num, width, rc.Height - num));
            brush2.Dispose();
        }

        private void DrawFrameCaption(Graphics g, Rectangle rc)
        {
            FrameLayoutInfo frameLayout = FrameLayout;
            Rectangle rc2 = frameLayout.CloseBox;
            DrawFrameButton(g, rc2, 0, CloseBox);
            if (ShowMaximizeBox && !HideCaptionButtons) DrawFrameButton(g, frameLayout.MaximizeBox, MaximizeButton, MaximizeBox);
            if (ShowMinimizeBox && !HideCaptionButtons) DrawFrameButton(g, frameLayout.MinimizeBox, MinimizeButton, MinimizeBox);
            if (IsMdiContainer)
            {
                Form activeMdiChild = ActiveMdiChild;
                if (activeMdiChild != null && activeMdiChild.WindowState == FormWindowState.Maximized)
                {
                    DrawFrameIcon(g, frameLayout.MdiIconBox);
                    DrawFrameButton(g, frameLayout.MdiCloseBox, 16, GetCloseBox(activeMdiChild));
                    DrawFrameButton(g, frameLayout.MdiMaximizeBox, 19, activeMdiChild.MaximizeBox);
                    DrawFrameButton(g, frameLayout.MdiMinimizeBox, 18, activeMdiChild.MinimizeBox);
                    DrawFrameButton(g, frameLayout.MdiHelpButton, 21, activeMdiChild.HelpButton);
                }
            }

            if (Application.StartupPath != null && FormBorderStyle == FormBorderStyle.FixedSingle)
            {
                string[] array = Application.StartupPath.Split('\\');
                if (array.Length > 0 && array[array.Length - 1] == "Office15") goto IL_0228;
                if (array.Length > 0 && array[array.Length - 1] == "Office14") goto IL_0228;
                if (array.Length > 0 && array[array.Length - 1] == "Office12") goto IL_0228;
                if (FormBorderStyle == FormBorderStyle.FixedSingle && CaptionBarHeight == 26 && !ShowMaximizeBox)
                    DrawFrameText(g, new Rectangle(frameLayout.TextBox.X, frameLayout.TextBox.Y, frameLayout.TextBox.Width, frameLayout.CloseBox.Height));
                else
                    DrawFrameText(g, frameLayout.TextBox);
            }
            else if (FormBorderStyle == FormBorderStyle.FixedSingle && CaptionBarHeight == 26 && !ShowMaximizeBox)
            {
                DrawFrameText(g, new Rectangle(frameLayout.TextBox.X, frameLayout.TextBox.Y, frameLayout.TextBox.Width, frameLayout.CloseBox.Height));
            }
            else
            {
                DrawFrameText(g, frameLayout.TextBox);
            }

            IL_0323:
            DrawFrameIcon(g, frameLayout.IconBox, frameLayout.TextBox.Right);
            return;
            IL_0228:
            DrawFrameText(g, frameLayout.TextBox);
            goto IL_0323;
        }

        private void DrawFrameIcon(Graphics g, Rectangle rc)
        {
            DrawFrameIcon(g, rc, 0);
        }

        private void DrawFrameIcon(Graphics g, Rectangle rc, int left)
        {
            Size size = new Size(16, 16);
            int num = 0;
            int num2 = SystemInformation.SmallIconSize.Height + 12;
            int num3 = WindowState == FormWindowState.Maximized ? 2 + TitlePadding : 0;
            int num4 = captionBarHeight * 20 / 100;
            if (num2 < TitleHeight)
            {
                if (g.DpiX > 120f)
                {
                    size = new Size(20, 20);
                    num = 4;
                }
                else if (g.DpiX > 96f)
                {
                    size = new Size(18, 18);
                    num = 2;
                }
            }
            else
            {
                size = new Size(16, rc.Height);
            }

            FrameLayoutInfo frameLayout = FrameLayout;
            if (rc.Width > 0 && rc.Height > 0 && Icon != null)
            {
                Icon icon = new Icon(Icon, rc.Size);
                Rectangle rect = rc;
                if (icon != null)
                {
                    SolidBrush solidBrush = new SolidBrush(MetroColor);
                    int width = TextRenderer.MeasureText(Text, Font).Width;
                    if (IsRightToLeft) rc.X = Width - rc.Width;
                    Rectangle textBox;
                    if (IconTextRelation == LeftRightAlignment.Left && IconAlign == CaptionAlign)
                    {
                        if (IconAlign == HorizontalAlignment.Right)
                        {
                            textBox = frameLayout.TextBox;
                            if (textBox.Right - (width + 45) <= 0)
                            {
                                rect = new Rectangle(0, WindowState == FormWindowState.Maximized ? TitlePadding : 0, 24, TitleHeight);
                                g.FillRectangle(solidBrush, rect);
                                switch (CaptionVerticalAlignment)
                                {
                                    case VerticalAlignment.Top:
                                        switch (WindowState)
                                        {
                                            case FormWindowState.Normal:
                                                rect = new Rectangle(5, 3, size.Width, size.Height);
                                                break;
                                            case FormWindowState.Maximized:
                                                rect = new Rectangle(5, 9, size.Width, size.Height);
                                                break;
                                        }

                                        break;
                                    case VerticalAlignment.Center:
                                        rect = new Rectangle(5, rc.Y, size.Width, size.Height);
                                        break;
                                    case VerticalAlignment.Bottom:
                                        rect = new Rectangle(5, CaptionBarHeight - IconBounds.Height - 3, size.Width, size.Height);
                                        break;
                                }
                            }
                            else
                            {
                                rect = new Rectangle(frameLayout.TextBox.Right - (width + 45), 0, 24, TitleHeight);
                                if (RightToLeftLayout && RightToLeft == RightToLeft.Yes) rect = new Rectangle(Width - (frameLayout.TextBox.Right - (width + 45)) - 26, 0, 24, TitleHeight);
                                rect.Y += WindowState == FormWindowState.Maximized ? TitlePadding : 0;
                                g.FillRectangle(solidBrush, rect);
                                switch (CaptionVerticalAlignment)
                                {
                                    case VerticalAlignment.Top:
                                        switch (WindowState)
                                        {
                                            case FormWindowState.Normal:
                                                rect = new Rectangle(frameLayout.TextBox.Right - (width + 40), 3, size.Width, size.Height);
                                                break;
                                            case FormWindowState.Maximized:
                                                rect = new Rectangle(frameLayout.TextBox.Right - (width + 40), 9, size.Width, size.Height);
                                                break;
                                        }

                                        break;
                                    case VerticalAlignment.Center:
                                        rect = new Rectangle(frameLayout.TextBox.Right - (width + 40), rc.Y, size.Width, size.Height);
                                        break;
                                    case VerticalAlignment.Bottom:
                                        rect = new Rectangle(frameLayout.TextBox.Right - (width + 40), CaptionBarHeight - IconBounds.Height - 3, size.Width, size.Height);
                                        break;
                                }
                            }
                        }
                        else if (IconAlign == HorizontalAlignment.Center)
                        {
                            if (Width / 2 - (width - 10) <= 0)
                            {
                                rect = new Rectangle(0, WindowState == FormWindowState.Maximized ? TitlePadding : 0, 24, TitleHeight);
                                g.FillRectangle(solidBrush, rect);
                                switch (CaptionVerticalAlignment)
                                {
                                    case VerticalAlignment.Top:
                                        switch (WindowState)
                                        {
                                            case FormWindowState.Normal:
                                                rect = new Rectangle(5, 3, size.Width, size.Height);
                                                break;
                                            case FormWindowState.Maximized:
                                                rect = new Rectangle(5, 9, size.Width, size.Height);
                                                break;
                                        }

                                        break;
                                    case VerticalAlignment.Center:
                                        rect = new Rectangle(5, rc.Y, size.Width, size.Height);
                                        break;
                                    case VerticalAlignment.Bottom:
                                        rect = new Rectangle(5, CaptionBarHeight - IconBounds.Height - 3, size.Width, size.Height);
                                        break;
                                }
                            }
                            else
                            {
                                rect = new Rectangle(Width / 2 - (width - 10), 0, 24, TitleHeight);
                                if (RightToLeftLayout && RightToLeft == RightToLeft.Yes) rect = new Rectangle(Width - (Width / 2 - (width - 10) + 26), 0, 24, TitleHeight);
                                rect.Y += WindowState == FormWindowState.Maximized ? TitlePadding : 0;
                                g.FillRectangle(solidBrush, rect);
                                switch (CaptionVerticalAlignment)
                                {
                                    case VerticalAlignment.Top:
                                        switch (WindowState)
                                        {
                                            case FormWindowState.Normal:
                                                rect = new Rectangle(Width / 2 - (width - 15), 3, size.Width, size.Height);
                                                break;
                                            case FormWindowState.Maximized:
                                                rect = new Rectangle(Width / 2 - (width - 15), 9, size.Width, size.Height);
                                                break;
                                        }

                                        break;
                                    case VerticalAlignment.Center:
                                        rect = new Rectangle(Width / 2 - (width - 15), rc.Y, size.Width, size.Height);
                                        break;
                                    case VerticalAlignment.Bottom:
                                        rect = new Rectangle(Width / 2 - (width - 15), CaptionBarHeight - IconBounds.Height - 3, size.Width, size.Height);
                                        break;
                                }
                            }
                        }
                        else if (IconAlign == HorizontalAlignment.Left)
                        {
                            if (WindowState == FormWindowState.Maximized)
                            {
                                rect = new Rectangle(5, WindowState == FormWindowState.Maximized ? TitlePadding : 0, 24, TitleHeight);
                                g.FillRectangle(solidBrush, rect);
                                switch (CaptionVerticalAlignment)
                                {
                                    case VerticalAlignment.Top:
                                        switch (WindowState)
                                        {
                                            case FormWindowState.Normal:
                                                rect = new Rectangle(12, 3, size.Width, size.Height);
                                                break;
                                            case FormWindowState.Maximized:
                                                rect = new Rectangle(12, 9, size.Width, size.Height);
                                                break;
                                        }

                                        break;
                                    case VerticalAlignment.Center:
                                        rect = new Rectangle(12, rc.Y, size.Width, size.Height);
                                        break;
                                    case VerticalAlignment.Bottom:
                                        rect = new Rectangle(12, CaptionBarHeight - IconBounds.Height - 3, size.Width, size.Height);
                                        break;
                                }
                            }
                            else
                            {
                                rect = new Rectangle(0, 0, 24, TitleHeight);
                                switch (RightToLeftLayout)
                                {
                                    case true:
                                        rect = new Rectangle(Width - 24, 0, 24, TitleHeight);
                                        break;
                                    case false:
                                        rect = new Rectangle(0, 0, 24, TitleHeight);
                                        break;
                                }

                                rect.Y += WindowState == FormWindowState.Maximized ? TitlePadding : 0;
                                g.FillRectangle(solidBrush, rect);
                                switch (CaptionVerticalAlignment)
                                {
                                    case VerticalAlignment.Top:
                                        switch (WindowState)
                                        {
                                            case FormWindowState.Normal:
                                                rect = new Rectangle(5, 3, size.Width, size.Height);
                                                break;
                                            case FormWindowState.Maximized:
                                                rect = new Rectangle(5, 9, size.Width, size.Height);
                                                break;
                                        }

                                        break;
                                    case VerticalAlignment.Center:
                                        rect = new Rectangle(5, rc.Y, size.Width, 16);
                                        break;
                                    case VerticalAlignment.Bottom:
                                        rect = new Rectangle(5, CaptionBarHeight - IconBounds.Height - 3, size.Width, size.Height);
                                        break;
                                }
                            }
                        }
                    }
                    else if (IconAlign == HorizontalAlignment.Right)
                    {
                        if (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
                        {
                            rect = new Rectangle(70, WindowState == FormWindowState.Maximized ? TitlePadding : 0, 24, TitleHeight);
                            g.FillRectangle(solidBrush, rect);
                            switch (CaptionVerticalAlignment)
                            {
                                case VerticalAlignment.Top:
                                    switch (WindowState)
                                    {
                                        case FormWindowState.Normal:
                                            rect = new Rectangle(left - 20, 3, size.Width, size.Height);
                                            break;
                                        case FormWindowState.Maximized:
                                            rect = new Rectangle(left - 20, 9, size.Width, size.Height);
                                            break;
                                    }

                                    break;
                                case VerticalAlignment.Center:
                                    rect = new Rectangle(left - 20, rc.Y, size.Width, size.Height);
                                    break;
                                case VerticalAlignment.Bottom:
                                    rect = new Rectangle(left - 20, CaptionBarHeight - IconBounds.Height - 3, size.Width, size.Height);
                                    break;
                            }
                        }
                        else
                        {
                            rect = new Rectangle(left - 24, WindowState == FormWindowState.Maximized ? TitlePadding : 0, 24, TitleHeight);
                            g.FillRectangle(solidBrush, rect);
                            switch (CaptionVerticalAlignment)
                            {
                                case VerticalAlignment.Top:
                                    switch (WindowState)
                                    {
                                        case FormWindowState.Normal:
                                            rect = new Rectangle(left - 20, 3, size.Width, size.Height);
                                            break;
                                        case FormWindowState.Maximized:
                                            rect = new Rectangle(left - 20, 9, size.Width, size.Height);
                                            break;
                                    }

                                    break;
                                case VerticalAlignment.Center:
                                    rect = new Rectangle(left - 20, rc.Y, size.Width, size.Height);
                                    break;
                                case VerticalAlignment.Bottom:
                                    rect = new Rectangle(left - 20, CaptionBarHeight - IconBounds.Height - 3, size.Width, size.Height);
                                    break;
                            }
                        }
                    }
                    else if (IconAlign == HorizontalAlignment.Center)
                    {
                        if (Width / 2 + (width - 45) >= left - 24)
                        {
                            rect = new Rectangle(left - 24, WindowState == FormWindowState.Maximized ? TitlePadding : 0, 24, TitleHeight);
                            g.FillRectangle(solidBrush, rect);
                            switch (CaptionVerticalAlignment)
                            {
                                case VerticalAlignment.Top:
                                    switch (WindowState)
                                    {
                                        case FormWindowState.Normal:
                                            rect = new Rectangle(left - 20, 3, size.Width, size.Height);
                                            break;
                                        case FormWindowState.Maximized:
                                            rect = new Rectangle(left - 20, 9, size.Width, size.Height);
                                            break;
                                    }

                                    break;
                                case VerticalAlignment.Center:
                                    rect = new Rectangle(left - 20, rc.Y, size.Width, size.Height);
                                    break;
                                case VerticalAlignment.Bottom:
                                    rect = new Rectangle(left - 20, CaptionBarHeight - IconBounds.Height - 3, size.Width, size.Height);
                                    break;
                            }
                        }
                        else
                        {
                            rect = new Rectangle(Width / 2 + (width - 25), 0, 24, TitleHeight);
                            if (RightToLeft == RightToLeft.Yes && RightToLeftLayout) rect = new Rectangle(Width - (Width / 2 + (width - 25) + 26), 0, 24, TitleHeight);
                            rect.Y += WindowState == FormWindowState.Maximized ? TitlePadding : 0;
                            g.FillRectangle(solidBrush, rect);
                            switch (CaptionVerticalAlignment)
                            {
                                case VerticalAlignment.Top:
                                    switch (WindowState)
                                    {
                                        case FormWindowState.Normal:
                                            rect = new Rectangle(Width / 2 + (width - 20), 3, size.Width, size.Height);
                                            break;
                                        case FormWindowState.Maximized:
                                            rect = new Rectangle(Width / 2 + (width - 20), 9, size.Width, size.Height);
                                            break;
                                    }

                                    break;
                                case VerticalAlignment.Center:
                                    rect = new Rectangle(Width / 2 + (width - 20), rc.Y, size.Width, size.Height);
                                    break;
                                case VerticalAlignment.Bottom:
                                    rect = new Rectangle(Width / 2 + (width - 20), CaptionBarHeight - IconBounds.Height - 3, size.Width, size.Height);
                                    break;
                            }
                        }
                    }
                    else if (IconAlign == HorizontalAlignment.Left && CaptionAlign == IconAlign)
                    {
                        if (frameLayout.TextBox.Left + width >= left - 24)
                        {
                            rect = new Rectangle(left - 24, WindowState == FormWindowState.Maximized ? TitlePadding : 0, 24, TitleHeight);
                            g.FillRectangle(solidBrush, rect);
                            rect = new Rectangle(left - 20, rc.Y, size.Width, size.Height);
                        }
                        else
                        {
                            rect = new Rectangle(frameLayout.TextBox.Left + width, 0, 24, TitleHeight);
                            if (RightToLeft == RightToLeft.Yes && RightToLeftLayout) rect = new Rectangle(Width - (frameLayout.TextBox.Left + width) - 45, 0, 24, TitleHeight);
                            rect.Y += WindowState == FormWindowState.Maximized ? TitlePadding : 0;
                            g.FillRectangle(solidBrush, rect);
                            switch (CaptionVerticalAlignment)
                            {
                                case VerticalAlignment.Top:
                                    switch (WindowState)
                                    {
                                        case FormWindowState.Normal:
                                            rect = new Rectangle(frameLayout.TextBox.Left + width + 5, 3, size.Width, size.Height);
                                            break;
                                        case FormWindowState.Maximized:
                                            rect = new Rectangle(frameLayout.TextBox.Left + width + 5, 9, size.Width, size.Height);
                                            break;
                                    }

                                    break;
                                case VerticalAlignment.Center:
                                    rect = new Rectangle(frameLayout.TextBox.Left + width + 5, rc.Y, size.Width, size.Height);
                                    break;
                                case VerticalAlignment.Bottom:
                                    rect = new Rectangle(frameLayout.TextBox.Left + width + 5, CaptionBarHeight - IconBounds.Height - 3, size.Width, size.Height);
                                    break;
                            }

                            bool rightToLeftLayout = RightToLeftLayout;
                            if (rightToLeftLayout)
                            {
                                textBox = frameLayout.TextBox;
                                int x = textBox.Left + width + 24;
                                int num5 = CaptionBarHeight;
                                textBox = IconBounds;
                                rect = new Rectangle(x, num5 - textBox.Height - 3, size.Width, size.Height);
                            }
                        }
                    }
                    else if (WindowState == FormWindowState.Maximized)
                    {
                        rect = new Rectangle(5, 0, 24, TitleHeight);
                        switch (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
                        {
                            case true:
                                rect = new Rectangle(Width - 31, 0, 24, TitleHeight);
                                break;
                            case false:
                                rect = new Rectangle(5, 0, 24, TitleHeight);
                                break;
                        }

                        rect.Y += TitlePadding;
                        g.FillRectangle(solidBrush, rect);
                        switch (CaptionVerticalAlignment)
                        {
                            case VerticalAlignment.Top:
                                switch (WindowState)
                                {
                                    case FormWindowState.Normal:
                                        rect = new Rectangle(12, 3, size.Width, size.Height);
                                        break;
                                    case FormWindowState.Maximized:
                                        rect = new Rectangle(12, 9, size.Width, size.Height);
                                        break;
                                }

                                break;
                            case VerticalAlignment.Center:
                                rect = new Rectangle(12, rc.Y, size.Width, size.Height);
                                break;
                            case VerticalAlignment.Bottom:
                            {
                                int num6 = CaptionBarHeight;
                                textBox = IconBounds;
                                rect = new Rectangle(12, num6 - textBox.Height - 3, size.Width, size.Height);
                                break;
                            }
                        }
                    }
                    else
                    {
                        rect = new Rectangle(0, 0, 24, TitleHeight);
                        switch (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
                        {
                            case true:
                                rect = new Rectangle(Width - 24, 0, 24, TitleHeight);
                                break;
                            case false:
                                rect = new Rectangle(0, 0, 24, TitleHeight);
                                break;
                        }

                        rect.Y += WindowState == FormWindowState.Maximized ? TitlePadding : 0;
                        g.FillRectangle(solidBrush, rect);
                        switch (CaptionVerticalAlignment)
                        {
                            case VerticalAlignment.Top:
                                switch (WindowState)
                                {
                                    case FormWindowState.Normal:
                                        rect = new Rectangle(5, 3, size.Width, size.Height);
                                        break;
                                    case FormWindowState.Maximized:
                                        rect = new Rectangle(5, 9, size.Width, size.Height);
                                        break;
                                }

                                break;
                            case VerticalAlignment.Center:
                                rect = new Rectangle(5, rc.Y, size.Width, size.Height);
                                break;
                            case VerticalAlignment.Bottom:
                            {
                                int num7 = CaptionBarHeight;
                                textBox = IconBounds;
                                rect = new Rectangle(5, num7 - textBox.Height - 3, size.Width, size.Height);
                                break;
                            }
                        }
                    }

                    if (num2 < TitleHeight)
                    {
                        rc.Y += WindowState == FormWindowState.Maximized ? TitlePadding : 0;
                        if (g.DpiX > 120f)
                        {
                            IconBounds = new Rectangle(rect.X - num + 1, rc.Y - num, size.Width + num, size.Height + num);
                            g.DrawIcon(icon, new Rectangle(rect.X - num + 1, rc.Y - num, size.Width + num, size.Height + num));
                        }
                        else if (g.DpiX > 96f)
                        {
                            IconBounds = new Rectangle(rect.X - num - 1, rc.Y - num, size.Width + num, size.Height + num);
                            g.DrawIcon(icon, new Rectangle(rect.X - num - 1, rc.Y - num, size.Width + num, size.Height + num));
                        }
                        else
                        {
                            g.DrawIcon(icon, new Rectangle(rect.X, rc.Y, size.Width, size.Height));
                            IconBounds = new Rectangle(rect.X, rc.Y, size.Width, size.Height);
                        }
                    }
                    else
                    {
                        g.DrawIcon(icon, new Rectangle(rect.X, num4 + num3, size.Width, size.Height));
                    }

                    solidBrush.Dispose();
                }
            }
        }

        private void DrawFrameButton(Graphics g, Rectangle rc, int buttonId, bool bEnabled)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Image buttonImage = GetButtonImage(buttonId);
                int num = SystemInformation.SmallIconSize.Height + 12;
                if (buttonImage != null)
                {
                    Size size = buttonImage.Size;
                    int num2 = rc.X + (rc.Width - size.Width) / 2 - (IsRightToLeft ? 1 : 0);
                    int num3 = rc.Y + (rc.Height - size.Height) / 2 + (buttonId != 20 && buttonId != 21 ? 1 : 0);
                    int num4 = WindowState == FormWindowState.Maximized ? 3 : 0;
                    int num5 = TitleHeight * 20 / 100;
                    Rectangle rect = new Rectangle(rc.X, num5 + num4, rc.Width, rc.Height);
                    int num6 = rc.X + (rect.Width - 9) / 2 - (IsRightToLeft ? 1 : 0);
                    int num7 = rect.Y + (rect.Height - 9) / 2 + (buttonId != 20 && buttonId != 21 ? 1 : 0);
                    if (bEnabled)
                    {
                        Color color = CaptionButtonColor;
                        Border = Color.Transparent;
                        if (HighlightedButton == buttonId)
                        {
                            if (PressedButton == buttonId)
                            {
                                DrawFrameButtonBackgroundPressed(g, ref rc);
                            }
                            else
                            {
                                DrawFrameButtonBackgroundSelected(g, ref rc);
                                color = !ShowMouseOver ? CaptionButtonHoverColor : Color.White;
                                Border = CaptionButtonColor;
                            }
                        }

                        Pen pen = new Pen(color, 2f);
                        switch (buttonId)
                        {
                            case 0:
                            case 16:
                            {
                                bool flag = false;
                                int num8 = 0;
                                int num9 = 0;
                                int num10 = 0;

                                if (!hasCaptionHeight || num < TitleHeight)
                                {
                                    if (DpiAware.GetCurrentDpi() > 96f && flag)
                                    {
                                        g.DrawLine(pen, new Point(rc.X + num10 - 1, num9 + num8 + 1), new Point(rc.X + num10 * 3 - 1, num9 + num8 * 3 + 1));
                                        g.DrawLine(pen, new Point(rc.X + num10 - 1, num9 + num8 * 3 + 1), new Point(rc.X + num10 * 3 - 1, num9 + num8 + 1));
                                    }
                                    else if (g.DpiX > 120f)
                                    {
                                        g.DrawLine(pen, new Point(rc.X + 2, rc.Y + 4), new Point(rc.X + 18, rc.Y + 12 + 8));
                                        g.DrawLine(pen, new Point(rc.X + 2, rc.Y + 12 + 8), new Point(rc.X + 18, rc.Y + 4));
                                    }
                                    else if (g.DpiX > 96f)
                                    {
                                        g.DrawLine(pen, new Point(rc.X + 4, rc.Y + 5), new Point(rc.X + 14, rc.Y + 12 + 5));
                                        g.DrawLine(pen, new Point(rc.X + 4, rc.Y + 12 + 5), new Point(rc.X + 14, rc.Y + 5));
                                    }
                                    else if (WindowState == FormWindowState.Maximized)
                                    {
                                        g.DrawLine(pen, new Point(num2 + 1, num3), new Point(num2 + 7, num3 + 7));
                                        g.DrawLine(pen, new Point(num2 + 1, num3 + 7), new Point(num2 + 7, num3));
                                    }
                                    else
                                    {
                                        g.DrawLine(pen, new Point(num2 + 1, num3), new Point(num2 + 7, num3 + 7));
                                        g.DrawLine(pen, new Point(num2 + 1, num3 + 7), new Point(num2 + 7, num3));
                                    }
                                }
                                else
                                {
                                    g.DrawLine(pen, new Point(num6 + 1, num7), new Point(num6 + 7, num7 + 7));
                                    g.DrawLine(pen, new Point(num6 + 1, num7 + 7), new Point(num6 + 7, num7));
                                }

                                break;
                            }
                            case 1:
                                if (!hasCaptionHeight || num < TitleHeight)
                                {
                                    if (g.DpiX > 120f)
                                    {
                                        pen = new Pen(color);
                                        Rectangle rect4 = new Rectangle(rc.X + 3, rc.Y + 6 - adjustvalueForDPI + 5, 8 + adjustvalueForDPI + 3, 6 + adjustvalueForDPI + 2);
                                        g.DrawRectangle(pen, rect4);
                                        g.DrawLine(pen, new Point(rc.X + 4, rc.Y + 7 - adjustvalueForDPI + 5), new Point(rc.X + 14 + adjustvalueForDPI, rc.Y + 7 - adjustvalueForDPI + 5));
                                        g.DrawLine(pen, new Point(rc.X + 4, rc.Y + 7 - adjustvalueForDPI + 6), new Point(rc.X + 14 + adjustvalueForDPI, rc.Y + 7 - adjustvalueForDPI + 6));
                                    }
                                    else if (g.DpiX > 96f)
                                    {
                                        pen = new Pen(color);
                                        Rectangle rect5 = new Rectangle(rc.X + 4, rc.Y + 6 - adjustvalueForDPI + 5, 8 + adjustvalueForDPI, 6 + adjustvalueForDPI);
                                        g.DrawRectangle(pen, rect5);
                                        g.DrawLine(pen, new Point(rc.X + 4, rc.Y + 7 - adjustvalueForDPI + 5), new Point(rc.X + 12 + adjustvalueForDPI, rc.Y + 7 - adjustvalueForDPI + 5));
                                    }
                                    else
                                    {
                                        pen = new Pen(color);
                                        Rectangle rect6 = new Rectangle(rc.X + 5, rc.Y + 6, 8, 6);
                                        g.DrawRectangle(pen, rect6);
                                        g.DrawLine(pen, new Point(rc.X + 5, rc.Y + 7), new Point(rc.X + 12, rc.Y + 7));
                                    }
                                }
                                else
                                {
                                    pen = new Pen(color);
                                    Rectangle rect7 = new Rectangle(num6 + 1, num7 + 1, 8, 6);
                                    g.DrawRectangle(pen, rect7);
                                    g.DrawLine(pen, new Point(num6 + 1, num7 + 2), new Point(num6 + 9, num7 + 2));
                                }

                                break;
                            case 2:
                            case 18:
                                if (!hasCaptionHeight || num < TitleHeight)
                                {
                                    if (g.DpiX > 120f)
                                    {
                                        g.DrawLine(pen, new Point(rc.X + 3, rc.Y + 11 + 4), new Point(rc.X + 16 + adjustvalueForDPI, rc.Y + 11 + 4));
                                    }
                                    else if (g.DpiX > 96f)
                                    {
                                        if (WindowState == FormWindowState.Normal)
                                            g.DrawLine(pen, new Point(rc.X + 6, rc.Y + 11 + 2), new Point(rc.X + 13 + adjustvalueForDPI, rc.Y + 11 + 2));
                                        else if (WindowState == FormWindowState.Maximized) g.DrawLine(pen, new Point(rc.X + 5, rc.Y + 11 + 1), new Point(rc.X + 12 + adjustvalueForDPI, rc.Y + 11 + 1));
                                    }
                                    else if (WindowState == FormWindowState.Maximized)
                                    {
                                        g.DrawLine(pen, new Point(rc.X + 6, rc.Y + 10), new Point(rc.X + 13, rc.Y + 10));
                                    }
                                    else
                                    {
                                        g.DrawLine(pen, new Point(rc.X + 6, rc.Y + 10), new Point(rc.X + 13, rc.Y + 10));
                                    }
                                }
                                else
                                {
                                    g.DrawLine(pen, new Point(num6 + 1, num7 + 5), new Point(num6 + 7, num7 + 5));
                                }

                                break;
                            case 3:
                            case 19:
                                if (!hasCaptionHeight || num < TitleHeight)
                                {
                                    if (g.DpiX > 120f)
                                    {
                                        pen = new Pen(color);
                                        Rectangle rect2 = new Rectangle(rc.X + 1, rc.Y + 6 - adjustvalueForDPI + 5, 6 + adjustvalueForDPI + 2, 5 + adjustvalueForDPI + 2);
                                        g.DrawRectangle(pen, rect2);
                                        g.DrawLine(pen, new Point(rc.X + 1, rc.Y + 6 - adjustvalueForDPI + 5 + 1), new Point(rect2.X + 12, rc.Y + 6 - adjustvalueForDPI + 5 + 1));
                                        g.DrawLine(pen, new Point(rc.X + 8, rc.Y + 3), new Point(rc.X + 20, rc.Y + 3));
                                        g.DrawLine(pen, new Point(rc.X + 8, rc.Y + 4), new Point(rc.X + 20, rc.Y + 4));
                                        g.DrawLine(pen, new Point(rc.X + 8, rc.Y + 4), new Point(rc.X + 8, rc.Y + 6));
                                        g.DrawLine(pen, new Point(rc.X + 20, rc.Y + 4), new Point(rc.X + 20, rc.Y + 14));
                                        g.DrawLine(pen, new Point(rc.X + 20, rc.Y + 14), new Point(rc.X + 13, rc.Y + 14));
                                    }
                                    else if (g.DpiX > 96f)
                                    {
                                        pen = new Pen(color);
                                        Rectangle rect3 = new Rectangle(rc.X + 3, rc.Y + 6 - adjustvalueForDPI + 5, 6 + adjustvalueForDPI, 5 + adjustvalueForDPI);
                                        g.DrawRectangle(pen, rect3);
                                        g.DrawLine(pen, new Point(rc.X + 3, rc.Y + 6 - adjustvalueForDPI + 5 + 1), new Point(rect3.X + 10, rc.Y + 6 - adjustvalueForDPI + 5 + 1));
                                        g.DrawLine(pen, new Point(rc.X + 7, rc.Y + 3), new Point(rc.X + 17, rc.Y + 3));
                                        g.DrawLine(pen, new Point(rc.X + 7, rc.Y + 4), new Point(rc.X + 17, rc.Y + 4));
                                        g.DrawLine(pen, new Point(rc.X + 7, rc.Y + 4), new Point(rc.X + 7, rc.Y + 6));
                                        g.DrawLine(pen, new Point(rc.X + 17, rc.Y + 4), new Point(rc.X + 17, rc.Y + 12));
                                        g.DrawLine(pen, new Point(rc.X + 17, rc.Y + 12), new Point(rc.X + 13, rc.Y + 12));
                                    }
                                    else
                                    {
                                        pen = new Pen(color);
                                        Rectangle empty = Rectangle.Empty;
                                        g.DrawLine(pen, new Point(rc.X + 8, rc.Y + 5), new Point(rc.X + 13, rc.Y + 5));
                                        g.DrawLine(pen, new Point(rc.X + 8, rc.Y + 6), new Point(rc.X + 13, rc.Y + 6));
                                        g.DrawLine(pen, new Point(rc.X + 13, rc.Y + 5), new Point(rc.X + 13, rc.Y + 10));
                                        g.DrawLine(pen, new Point(rc.X + 11, rc.Y + 10), new Point(rc.X + 13, rc.Y + 10));
                                        empty = new Rectangle(rc.X + 5, rc.Y + 7, 6, 5);
                                        g.DrawRectangle(pen, empty);
                                        g.DrawLine(pen, new Point(rc.X + 5, rc.Y + 7), new Point(rc.X + 10, rc.Y + 7));
                                    }
                                }
                                else
                                {
                                    pen = new Pen(color);
                                    Rectangle empty2 = Rectangle.Empty;
                                    g.DrawLine(pen, new Point(num6 + 2, num7 + 1), new Point(num6 + 8, num7 + 1));
                                    g.DrawLine(pen, new Point(num6 + 2, num7 + 2), new Point(num6 + 8, num7 + 2));
                                    g.DrawLine(pen, new Point(num6 + 8, num7 + 1), new Point(num6 + 8, num7 + 6));
                                    g.DrawLine(pen, new Point(num6 + 6, num7 + 6), new Point(num6 + 8, num7 + 6));
                                    empty2 = new Rectangle(num6 - 1, num7 + 3, 6, 5);
                                    g.DrawRectangle(pen, empty2);
                                }

                                break;
                            case 20:
                            case 21:
                                g.DrawImage(buttonImage, FrameLayout.HelpButton);
                                break;
                        }

                        if (g.DpiX > 120f)
                        {
                            rc.Width -= 5;
                        }
                        else if (g.DpiX > 96f)
                        {
                            rc.Width -= 2;
                            if (buttonId == 0) rc.X--;
                        }

                        if (EnableTouchMode && buttonId != 0) rc.X -= rc.Width / 4;
                        if (!hasCaptionHeight || num < TitleHeight)
                        {
                            if (DpiAware.GetCurrentDpi() > 96f && !hasCaptionHeight)
                            {
                                int num11 = FrameLayout.CaptionHeight / 6;
                                int y = (FrameLayout.CaptionHeight - num11 * 4) / 2;
                                g.DrawRectangle(rect: new Rectangle(rc.X, y, rc.Width, rc.Height), pen: new Pen(Border));
                            }
                            else
                            {
                                g.DrawRectangle(new Pen(Border), rc);
                            }
                        }
                        else
                        {
                            g.DrawRectangle(new Pen(Border), rect);
                        }

                        pen.Dispose();
                    }
                    else
                    {
                        Color lightGray = Color.LightGray;
                        Pen pen2 = new Pen(lightGray, 2f);
                        switch (buttonId)
                        {
                            case 2:
                                if (g.DpiX > 120f)
                                    g.DrawLine(pen2, new Point(rc.X + 3, rc.Y + 11 + 7), new Point(rc.X + 16 + adjustvalueForDPI, rc.Y + 11 + 7));
                                else if (g.DpiX > 96f)
                                    g.DrawLine(pen2, new Point(rc.X + 6, rc.Y + 11 + 5), new Point(rc.X + 13 + adjustvalueForDPI, rc.Y + 11 + 5));
                                else
                                    g.DrawLine(pen2, new Point(rc.X + 6, rc.Y + 12), new Point(rc.X + 13, rc.Y + 12));
                                break;
                            case 1:
                                if (g.DpiX > 120f)
                                {
                                    pen2 = new Pen(lightGray);
                                    Rectangle rect9 = new Rectangle(rc.X + 2, rc.Y + 6 - adjustvalueForDPI + 5, 8 + adjustvalueForDPI + 3, 6 + adjustvalueForDPI + 2);
                                    g.DrawRectangle(pen2, rect9);
                                    g.DrawLine(pen2, new Point(rc.X + 2, rc.Y + 7 - adjustvalueForDPI + 5), new Point(rc.X + 12 + adjustvalueForDPI, rc.Y + 7 - adjustvalueForDPI + 5));
                                    g.DrawLine(pen2, new Point(rc.X + 2, rc.Y + 7 - adjustvalueForDPI + 6), new Point(rc.X + 12 + adjustvalueForDPI, rc.Y + 7 - adjustvalueForDPI + 6));
                                }
                                else if (g.DpiX > 96f)
                                {
                                    pen2 = new Pen(lightGray);
                                    Rectangle rect10 = new Rectangle(rc.X + 5, rc.Y + 6 - adjustvalueForDPI + 5, 8 + adjustvalueForDPI, 6 + adjustvalueForDPI);
                                    g.DrawRectangle(pen2, rect10);
                                    g.DrawLine(pen2, new Point(rc.X + 5, rc.Y + 7 - adjustvalueForDPI + 5), new Point(rc.X + 12 + adjustvalueForDPI, rc.Y + 7 - adjustvalueForDPI + 5));
                                }
                                else
                                {
                                    pen2 = new Pen(lightGray);
                                    Rectangle rect11 = new Rectangle(rc.X + 5, rc.Y + 6, 8, 6);
                                    g.DrawRectangle(pen2, rect11);
                                    g.DrawLine(pen2, new Point(rc.X + 5, rc.Y + 7), new Point(rc.X + 12, rc.Y + 7));
                                }

                                break;
                            default:
                                ControlPaint.DrawImageDisabled(g, buttonImage, num2, num3, Color.White);
                                break;
                        }

                        pen2.Dispose();
                    }
                }
            }
        }

        private void DrawFrameButtonBackgroundSelected(Graphics g, ref Rectangle rc)
        {
            if (ShowMouseOver)
            {
                SolidBrush solidBrush = new SolidBrush(MetroColor);
                rc.Y = 0;
                rc.Height = TitleHeight;
                g.FillRectangle(solidBrush, rc);
                solidBrush.Dispose();
            }

            new ToolTip();
        }

        private void DrawFrameButtonBackgroundPressed(Graphics g, ref Rectangle rc)
        {
            if (ShowMouseOver)
            {
                SolidBrush solidBrush = new SolidBrush(BackColor);
                rc.Y = 0;
                rc.Height = TitleHeight;
                g.FillRectangle(solidBrush, rc);
                solidBrush.Dispose();
            }
        }

        private void DrawFrameText(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                string text = Text;
                try
                {
                    int lCID = InputLanguage.CurrentInputLanguage.Culture.LCID;
                    if (RightToLeft == RightToLeft.Yes && (lCID == 1037 || lCID == 13))
                    {
                        string empty = string.Empty;
                        if (text.EndsWith("."))
                        {
                            string[] array = text.Split('.');
                            empty = array[0];
                            text = "." + empty;
                        }
                        else if (text.EndsWith(".]"))
                        {
                            int startIndex = text.LastIndexOf('.');
                            empty = text.Remove(startIndex, 2);
                            text = "[." + empty;
                        }
                    }
                }
                catch (Exception t)
                {
                    Application.OnThreadException(t);
                }

                if (text != string.Empty)
                {
                    IntPtr hdc = g.GetHdc();
                    IntPtr captionFontInternal = CaptionFontInternal;
                    if (captionFontInternal != IntPtr.Zero)
                    {
                        int iBkMode = NativeMethods.SetBkMode(hdc, 1);
                        int num = 0;
                        num = NativeMethods.SetTextColor(hdc, ColorTranslator.ToWin32(CaptionForeColor));
                        IntPtr hObject = NativeMethods.SelectObject(hdc, captionFontInternal);
                        int num2 = 34848;
                        HorizontalAlignment captionAlign = CaptionAlign;
                        switch (CaptionVerticalAlignment)
                        {
                            case VerticalAlignment.Top:
                                num2 = 34848;
                                break;
                            case VerticalAlignment.Center:
                                num2 = 34852;
                                break;
                            case VerticalAlignment.Bottom:
                                num2 = 34856;
                                break;
                        }

                        HorizontalAlignment horizontalAlignment = captionAlign;
                        if (horizontalAlignment == HorizontalAlignment.Center)
                            num2 |= 1;
                        else if ((RightToLeft == RightToLeft.Yes && !IsRightToLeft) ^ (captionAlign == HorizontalAlignment.Right)) num2 |= 2;
                        NativeMethods.RECT lpRect = new NativeMethods.RECT(rc);
                        if (IconTextRelation == LeftRightAlignment.Left && IconAlign == CaptionAlign)
                        {
                            switch (captionAlign)
                            {
                                case HorizontalAlignment.Right:
                                    lpRect.right = lpRect.right;
                                    break;
                                case HorizontalAlignment.Center:
                                    if (!MaximizeBox && !MinimizeBox)
                                        lpRect.left += 82;
                                    else
                                        lpRect.left += 120;
                                    break;
                            }

                            if (WindowState == FormWindowState.Maximized)
                            {
                                lpRect.top += 2;
                                lpRect.left += 5;
                            }

                            NativeMethods.DrawText(hdc, text, text.Length, ref lpRect, num2);
                        }
                        else
                        {
                            if (captionAlign == HorizontalAlignment.Right && IconAlign == CaptionAlign)
                                lpRect.right -= 40;
                            else
                                switch (captionAlign)
                                {
                                    case HorizontalAlignment.Right:
                                        lpRect.right = lpRect.right;
                                        break;
                                    case HorizontalAlignment.Center:
                                        if (IconAlign != CaptionAlign)
                                        {
                                            lpRect.left = lpRect.left;
                                            break;
                                        }

                                        goto default;
                                    default:
                                        if (RightToLeft == RightToLeft.No)
                                        {
                                            switch (WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    lpRect.left -= 25;
                                                    break;
                                                case FormWindowState.Maximized:
                                                    lpRect.left -= 15;
                                                    lpRect.top += 3;
                                                    break;
                                            }

                                            if (lpRect.left < 5) lpRect.left = 5;
                                        }
                                        else
                                        {
                                            switch (RightToLeft)
                                            {
                                                case RightToLeft.No:
                                                    lpRect.right -= 25;
                                                    break;
                                                case RightToLeft.Yes:
                                                    lpRect.right += 25;
                                                    break;
                                            }
                                        }

                                        break;
                                }

                            NativeMethods.DrawText(hdc, text, text.Length, ref lpRect, num2);
                        }

                        NativeMethods.SelectObject(hdc, hObject);
                        NativeMethods.SetTextColor(hdc, num);
                        NativeMethods.SetBkMode(hdc, iBkMode);
                        NativeMethods.DeleteObject(captionFontInternal);
                    }

                    g.ReleaseHdc(hdc);
                }
            }
        }

        private void DrawFrameBorders(Graphics g, Rectangle rc)
        {
            SmoothingMode smoothingMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color empty = Color.Empty;
            empty = !(NativeMethods.GetForegroundWindow() != Handle) ? IsActive ? BorderColor : Color.FromArgb(197, 197, 197) : BorderColor;
            if (WindowState != FormWindowState.Maximized)
            {
                Pen pen = new Pen(empty, BorderThickness);
                Rectangle rect = new Rectangle(rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                g.DrawRectangle(pen, rect);
                pen.Dispose();
            }

            g.SmoothingMode = smoothingMode;
        }

        #endregion

        #region Natives

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFontIndirect(ref NativeMethods.LOGFONT lplf);

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        private static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        private static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4) return GetClassLongPtr64(hWnd, nIndex);
            return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
        }

        #endregion
    }
}