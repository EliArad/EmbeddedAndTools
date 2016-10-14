using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using marlie.TumbnailDotnet;

namespace TopazApp.TOPNAT
{
    public partial class ViewNatPicturesForm : Form
    {

        private ThumbnailController m_Controller;
        public event ThumbnailImageEventHandler OnImageSizeChanged;
        private ImageDialog m_ImageDialog;
      
        private ImageViewer m_ActiveImageViewer;
        

        private int ImageSize
        {
            get
            {
                return 280;
            }
        }

        public ViewNatPicturesForm(int testid)
        {
            InitializeComponent();

            m_AddImageDelegate = new DelegateAddImage(this.AddImage);

            m_Controller = new ThumbnailController();
            m_Controller.OnStart += new ThumbnailControllerEventHandler(m_Controller_OnStart);
            m_Controller.OnAdd += new ThumbnailControllerEventHandler(m_Controller_OnAdd);
            m_Controller.OnEnd += new ThumbnailControllerEventHandler(m_Controller_OnEnd);
            try
            {

                this.flowLayoutPanelMain.Controls.Clear();
                this.flowLayoutPanelMain1.Controls.Clear();
                m_Controller.AddFolder(testid);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }

        private void m_Controller_OnStart(object sender, ThumbnailControllerEventArgs e)
        {

        }

        private void m_Controller_OnAdd(object sender, ThumbnailControllerEventArgs e)
        {
            
            this.AddImage(e.ImageFilename, e.layout, e.description ,e.title);
            this.Invalidate();
        }

        private void m_Controller_OnEnd(object sender, ThumbnailControllerEventArgs e)
        {
            // thread safe
            if (this.InvokeRequired)
            {
                this.Invoke(new ThumbnailControllerEventHandler(m_Controller_OnEnd), sender, e);
            }
             
        }

        delegate void DelegateAddImage(string imageFilename, int layout, string desc, string title);
        private DelegateAddImage m_AddImageDelegate;

        private void AddImage(string imageFilename, int layout, string desc,  string title)
        {
            // thread safe
            if (this.InvokeRequired)
            {
                this.Invoke(m_AddImageDelegate, imageFilename, layout, desc, title);
            }
            else
            {
                int size = ImageSize;

                ImageViewer imageViewer = new ImageViewer();
                imageViewer.Dock = DockStyle.Bottom;
                imageViewer.LoadImage(imageFilename, 256, 256);
                imageViewer.Width = size;
                imageViewer.Height = size;
                imageViewer.Top = 5;
                imageViewer.SetPictureInfo(desc, title);
                imageViewer.IsThumbnail = true;
                imageViewer.MouseClick += new MouseEventHandler(imageViewer_MouseClick);
                //imageViewer.MouseEnter += new EventHandler(imageViewer_MouseEnter);
                //imageViewer.MouseLeave += new EventHandler(imageViewer_MouseLeave);

                this.OnImageSizeChanged += new ThumbnailImageEventHandler(imageViewer.ImageSizeChanged);

                if (layout == 0)
                    this.flowLayoutPanelMain.Controls.Add(imageViewer);
                else
                    this.flowLayoutPanelMain1.Controls.Add(imageViewer);
            }
        }
      
         
        private void imageViewer_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_ActiveImageViewer != null)
            {
                m_ActiveImageViewer.IsActive = false;
            }

            m_ActiveImageViewer = (ImageViewer)sender;
            m_ActiveImageViewer.IsActive = true;

            m_ImageDialog = new ImageDialog();
            m_ImageDialog.SetImage(m_ActiveImageViewer.ImageLocation);
            m_ImageDialog.ShowDialog();            
        }        
    }
}
