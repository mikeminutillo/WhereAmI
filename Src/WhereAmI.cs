using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace WhereAmI
{
    class WhereAmI : Canvas, IWpfTextViewMargin
    {
        public const string MarginName = "WhereAmI";
        private readonly IWpfTextView _textView;
        private bool _isDisposed;
        private readonly FileNameMonitor _monitor;

        public WhereAmI(IWpfTextView textView)
        {
            _textView = textView;
            _monitor = new FileNameMonitor(GetDocument());

            Height = 20;
            ClipToBounds = true;
            Background = new SolidColorBrush(Colors.DarkSlateGray);

            var label = new Label
            {
                Background = new SolidColorBrush(Colors.DarkSlateGray),
                Foreground = new SolidColorBrush(Colors.White),
            };

            label.SetBinding(ContentControl.ContentProperty, new Binding("FileName") { Source = _monitor });

            Children.Add(label);

            ToolTip = "Click to open in explorer";
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + _monitor.FileName + "\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        private ITextDocument GetDocument()
        {
            ITextDocument document;
            _textView.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document);
            return document;
        }


        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(MarginName);
        }


        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return ActualHeight;
            }
        }

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return _monitor.FileName != null;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return (marginName == MarginName) ? this : null;
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _monitor.Dispose();
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

    }
}
