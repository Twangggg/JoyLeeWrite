using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using JoyLeeWrite.Commands;
using JoyLeeWrite.Services;
using JoyLeeWrite.Models;

namespace JoyLeeWrite.ViewModels
{
    public class EditorToolbarViewModel : INotifyPropertyChanged
    {
        private readonly TextFormattingService _textService;
        

        private double _selectedFontSize = 16;
        public double SelectedFontSize
        {
            get => _selectedFontSize;
            set
            {
                if (_selectedFontSize != value)
                {
                    _selectedFontSize = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color _selectedFontColor = Colors.Black;
        public Color SelectedFontColor
        {
            get => _selectedFontColor;
            set
            {
                if (_selectedFontColor != value)
                {
                    _selectedFontColor = value;
                    OnPropertyChanged();
                    _textService.ApplyFontColor(value);
                }
            }
        }
       
        public ICommand ToggleBoldCommand { get; }
        public ICommand ToggleItalicCommand { get; }
        public ICommand ToggleUnderlineCommand { get; }
        public ICommand UpSizeCommand { get; }
        public ICommand DownSizeCommand { get; }
        public ICommand AlignLeftCommand { get; }
        public ICommand AlignCenterCommand { get; }
        public ICommand AlignRightCommand { get; }
        public ICommand AlignJustifyCommand { get; }

        public EditorToolbarViewModel(TextFormattingService textService, int seriesId, int chapterId)
        {
            _textService = textService;

            ToggleBoldCommand = new RelayCommand(_ => _textService.ToggleBold(), _ => _textService.CanFormat());
            ToggleItalicCommand = new RelayCommand(_ => _textService.ToggleItalic(), _ => _textService.CanFormat());
            ToggleUnderlineCommand = new RelayCommand(_ => _textService.ToggleUnderline(), _ => _textService.CanFormat());
            UpSizeCommand = new RelayCommand(_ => { SelectedFontSize = _textService.ChangeFontSize(+2); }, _ => _textService.CanFormat());
            DownSizeCommand = new RelayCommand(_ => { SelectedFontSize = _textService.ChangeFontSize(-2); }, _ => _textService.CanFormat());
            AlignLeftCommand = new RelayCommand(_ => _textService.Align(TextAlignment.Left), _ => _textService.CanFormat());
            AlignCenterCommand = new RelayCommand(_ => _textService.Align(TextAlignment.Center), _ => _textService.CanFormat());
            AlignRightCommand = new RelayCommand(_ => _textService.Align(TextAlignment.Right), _ => _textService.CanFormat());
            AlignJustifyCommand = new RelayCommand(_ => _textService.Align(TextAlignment.Justify), _ => _textService.CanFormat());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.B:
                        _textService.ToggleBold();
                        e.Handled = true;
                        break;
                    case Key.I:
                        _textService.ToggleItalic();
                        e.Handled = true;
                        break;
                    case Key.U:
                        _textService.ToggleUnderline();
                        e.Handled = true;
                        break;
                }
            }
        }
    }
}
