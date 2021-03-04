﻿using Avalonia;
using Avalonia.Collections;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using JazzNotes.Helpers;
using JazzNotes.Models;
using ReactiveUI;

namespace JazzNotes.ViewModels
{
    public class TranscriptionViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel mainViewModel;
        private readonly Transcription transcription;
        private readonly PdfHelper pdfHelper;

        private AvaloniaList<NotesEditorViewModel> noteVMs;
        private Bitmap image;
        private bool showNotes;
        private Cursor currentCursor;

        /// <summary>
        /// Create new transcription viewmodel.
        /// </summary>
        /// <param name="mainViewModel">Main viewmodel.</param>
        /// <param name="transcription">The transcription.</param>
        public TranscriptionViewModel(MainWindowViewModel mainViewModel, Transcription transcription, Bitmap image)
        {
            this.mainViewModel = mainViewModel;
            this.noteVMs = new AvaloniaList<NotesEditorViewModel>();
            this.transcription = transcription;
            this.pdfHelper = mainViewModel.PdfHelper;
            this.ShowNotes = true;
            this.Image = image;
            this.CurrentCursor = CursorHelper.ArrowCursor;
            foreach (var note in transcription.Notes)
            {
                var bmp = this.pdfHelper.GetSnip(note.Snip, this.transcription.FilePath);
                this.noteVMs.Add(new NotesEditorViewModel(bmp, note, this.pdfHelper));
            }
        }

        /// <summary>
        /// The current image for the transcription.
        /// </summary>
        public Bitmap Image
        {
            get => this.image;
            set => this.RaiseAndSetIfChanged(ref this.image, value);
        }

        /// <summary>
        /// The screen width.
        /// </summary>
        public double GridWidth => WindowHelper.MaxWidth;

        /// <summary>
        /// The notes viewmodels.
        /// </summary>
        public AvaloniaList<NotesEditorViewModel> NoteVMs
        {
            get => this.noteVMs;
            set => this.RaiseAndSetIfChanged(ref this.noteVMs, value);
        }

        /// <summary>
        /// Whether notes should be shown or not.
        /// </summary>
        public bool ShowNotes
        {
            get => this.showNotes;
            set => this.RaiseAndSetIfChanged(ref this.showNotes, value);
        }

        /// <summary>
        /// The current cursor.
        /// </summary>
        public Cursor CurrentCursor
        {
            get => this.currentCursor;
            set => this.RaiseAndSetIfChanged(ref this.currentCursor, value);
        }

        /// <summary>
        /// Add a new note.
        /// </summary>
        /// <param name="bounds">Size for the note.</param>
        /// <param name="visual">Visual bounds for note.</param>
        /// <param name="actual">Actual bounds for note.</param>
        public void AddNote(Rect bounds, Rect visual, Rect actual)
        {
            var widthD = pdfHelper.Width / bounds.Width;
            var heightD = pdfHelper.Height / bounds.Height;

            var snip = new Rect(actual.Left * widthD, actual.Top * heightD, actual.Width * widthD, actual.Height * heightD);
            var bmp = this.pdfHelper.GetSnip(snip, this.transcription.FilePath);

            var note = new Note(this.transcription, snip, new Size(visual.Width, visual.Height), new Thickness(visual.Left, visual.Top, 0, 0));

            this.transcription.AddNote(note);

            var noteVm = new NotesEditorViewModel(bmp, note, this.pdfHelper);

            this.NoteVMs.Add(noteVm);

            this.mainViewModel.Content = noteVm;
        }

        /// <summary>
        /// The note to delete.
        /// </summary>
        /// <param name="sender">The note</param>
        public void DeleteNote(NotesEditorViewModel vm)
        {
            this.NoteVMs.Remove(vm);
        }

        /// <summary>
        /// Open edit for note.
        /// </summary>
        /// <param name="vm">The note view model to open.</param>
        public void EditNote(NotesEditorViewModel vm)
        {
            this.mainViewModel.Content = vm;
        }

        /// <summary>
        /// Get noteditor viewmodel for note.
        /// </summary>
        /// <param name="note">The note to find.</param>
        /// <returns>The note viewmodel.</returns>
        public NotesEditorViewModel GetNoteViewModel(Note note)
        {
            foreach (var notevm in this.noteVMs)
            {
                if (notevm.Text == note.Text
                    && notevm.TranscriptionName == note.Transcription.Name
                    && notevm.Width == note.Size.Width
                    && notevm.Height == note.Size.Height
                    && notevm.Margin == note.Margin
                    && notevm.Color == note.Color)
                {
                    return notevm;
                }
            }
            return null;
        }
    }
}