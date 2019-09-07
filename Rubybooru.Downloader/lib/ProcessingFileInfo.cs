using System.ComponentModel;
using System.Runtime.CompilerServices;
using Rubybooru.Downloader.Annotations;

namespace Rubybooru.Downloader.lib
{
    public enum ProcessingState
    {
        Init,
        Fetching,
        Parsing,
        Saving,
        Error
    }

    public class ProcessingFileInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string File { get; set; }

        private ProcessingState _state;
        public ProcessingState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged(nameof(State));
            }
        }

        public ProcessingFileInfo(string file, ProcessingState state = ProcessingState.Init)
        {
            File = file;
            State = state;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}