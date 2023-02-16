using System;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/*
 * "O, sancta piva di Polonia, o sancta biera di Varca"
 *  ~ Clement VIII
 */

namespace AlkoCalc
{
	[Serializable()]
	public class Notes<T>
	{
		public T[] notes;
		private int last = 0;
		private int capacity = 10;
		public Notes()
		{
			notes = new T[capacity];
		}

		public void addNote(T note)
		{
			if (last >= notes.Length)
			{
				capacity += 5;
				T[] n = new T[capacity];
				for (int i = 0; i < notes.Length; i++)
					n[i] = notes[i];
				notes = n;
			}
			notes[last++] = note;
		}

		public int notesQuantity()
		{
			return last;
		}
		public T getNote(int i)
		{
			return notes[i];
		}

		public void deleteNote(T n)
		{
			int i = 0;
			for (; i < last; i++)
			{
				if (notes[i].Equals(n))
				{
					break;
				}
			}
			for (; i < last && i + 1 < notes.Length; i++)
			{
				notes[i] = notes[i + 1];
			}
			last--;
		}
	}

	[Serializable()]
	public class Note
	{
		private string title_;
		private string contents_;
		public string title
		{
			get { return title_; }
			set
			{
				title_ = value;
				lastEdited = DateTime.Now;
			}
		}
		public string contents
		{
			get { return contents_; }
			set
			{
				contents_ = value;
				lastEdited = DateTime.Now;
			}
		}
		public DateTime date;
		public DateTime lastEdited;

		public Note(string title, string contents)
		{
			this.title_ = title;
			this.contents_ = contents;
			date = DateTime.Now;
			lastEdited = DateTime.Now;
		}

	}

	public class NoteBox : Button
	{
		public Note note;
		public NoteBox(Note n)
		{
			note = n;
			this.Text = n.title;
			this.Width = n.title.Length * 50;
		}
	}

	public class NotesPanel : TableLayoutPanel
	{
		private Notes<Note> noteBox;
		private ArrayList buttons = new ArrayList();
		NoteControls noteControls = new NoteControls();
		Button newNote;
		private struct NoteControls
		{
			public TextBox title;
			public TextBox contents;
			public Button add;
			public NoteBox edit;
			public NoteBox delete;
			public Button cancel;
		}
		public NotesPanel(Button newNote, Notes<Note> loadedNotes)
		{
			initPanel();
			initNoteControls();
			noteBox = loadedNotes;
			this.newNote = newNote;
			this.AutoScroll = true;
			addNotes();
		}

		private void initPanel()
		{
			this.Width = 1064;
			this.Height = 362;
			this.Location = new System.Drawing.Point(6, 6);
			this.ColumnCount = 4;
			this.ColumnStyles.Clear();
			for (int i = 0; i < 4; i++)
			{
				ColumnStyle cs = new ColumnStyle(SizeType.Percent, 25);
				this.ColumnStyles.Add(cs);
			}
			foreach (ColumnStyle style in this.ColumnStyles)
			{
				style.SizeType = SizeType.Percent;
				style.Width = 25;
			}
			foreach (RowStyle style in this.RowStyles)
			{
				style.SizeType = SizeType.AutoSize;
			}
		}

		private void initNoteControls()
		{
			noteControls.add = new Button();
			noteControls.add.Text = "Add";
			noteControls.title = new TextBox();
			noteControls.contents = new TextBox();
			noteControls.contents.Multiline = true;
			noteControls.contents.Height = 300;
			noteControls.contents.Width = 500;
			noteControls.add.Click += addNewNote;
			noteControls.cancel = new Button();
			noteControls.cancel.Text = "Cancel";
			noteControls.cancel.Click += cancelNote;
		}

		private void addNotes()
		{
			for (int i = 0; i < noteBox.notesQuantity(); i++)
			{
				NoteBox box = new NoteBox(noteBox.getNote(i));
				box.Click += editNote;
				buttons.Add(box);
				this.Controls.Add(box);
			}
		}
		private void removeNotes()
		{
			for (int i = 0; i < buttons.Count; i++)
				this.Controls.Remove((NoteBox)buttons[i]);
			for (int i = 0; i < buttons.Count; i++)
				buttons.RemoveAt(i);
		}

		private void addNoteControls()
		{
			this.Controls.Add(noteControls.title);
			this.Controls.Add(noteControls.contents);
			this.Controls.Add(noteControls.cancel);
			newNote.Visible = false;
		}

		private void removeNoteControls()
		{
			this.Controls.Remove(noteControls.title);
			this.Controls.Remove(noteControls.contents);
			noteControls.title.Text = "";
			noteControls.contents.Text = "";
			if (noteControls.edit != null)
			{
				this.Controls.Remove(noteControls.edit);
				noteControls.edit = null;
			}
			if (noteControls.delete != null)
			{

				this.Controls.Remove(noteControls.delete);
				noteControls.delete = null;
			}
			if (this.Controls.Contains(noteControls.add))
				this.Controls.Remove(noteControls.add);
			if (this.Controls.Contains(noteControls.cancel))
				this.Controls.Remove(noteControls.cancel);
			newNote.Visible = true;
		}

		public void newNoteAction()
		{
			removeNotes();
			addNoteControls();
			this.Controls.Add(noteControls.add);
		}
		private void addNewNote(Object sender, EventArgs ea)
		{
			Note n = new Note(noteControls.title.Text, noteControls.contents.Text);
			noteBox.addNote(n);
			buttons.Add(new NoteBox(n));
			removeNoteControls();
			addNotes();
		}
		private void editNote(Object sender, EventArgs ea)
		{
			removeNotes();
			NoteBox nb = (NoteBox)sender;
			Note n = nb.note;
			noteControls.title.Text = n.title;
			noteControls.contents.Text = n.contents;
			noteControls.edit = new NoteBox(n);
			noteControls.edit.Text = "Edit";
			noteControls.edit.Click += saveEditedNote;

			noteControls.delete = new NoteBox(n);
			noteControls.delete.Text = "Delete";
			noteControls.delete.Click += deleteNote;
			addNoteControls();
			this.Controls.Add(noteControls.edit);
			this.Controls.Add(noteControls.delete);
		}

		private void cancelNote(object sender, EventArgs ea)
		{
			removeNoteControls();
			addNotes();
			newNote.Visible = true;

		}

		private void deleteNote(object sender, EventArgs ea)
		{
			NoteBox nb = (NoteBox)sender;
			Note n = nb.note;
			noteBox.deleteNote(n);
			removeNoteControls();
			addNotes();
		}
		private void saveEditedNote(object sender, EventArgs ea)
		{
			NoteBox nb = (NoteBox)sender;
			Note n = nb.note;
			n.title = noteControls.title.Text;
			n.contents = noteControls.contents.Text;
			removeNoteControls();
			addNotes();
		}
	}
}