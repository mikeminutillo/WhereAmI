using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Text;

namespace WhereAmI
{
    class FileNameMonitor : INotifyPropertyChanged, IDisposable
    {
        private string _fileName;
        private bool _isDisposed;
        private readonly ITextDocument _document;
        public event PropertyChangedEventHandler PropertyChanged;

        public FileNameMonitor(ITextDocument document)
        {
            if (document == null) return;

            _document = document;

            FileName = _document.FilePath;

            _document.FileActionOccurred += FileChanged;

        }

        private void FileChanged(object sender, TextDocumentFileActionEventArgs args)
        {
            FileName = args.FilePath;
        }

        public virtual void Dispose()
        {
            if (_isDisposed) return;
            if (_document != null)
                _document.FileActionOccurred -= FileChanged;
            _isDisposed = true;
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName == value) return;
                _fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var temp = PropertyChanged;
            if (temp != null)
                temp(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
