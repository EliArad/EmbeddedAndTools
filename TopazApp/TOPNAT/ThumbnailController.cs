using System;
using System.Drawing;
using System.Threading;
using System.IO;
using TopazApp;
using System.Collections.Generic; 



namespace marlie.TumbnailDotnet
{
    public class ThumbnailControllerEventArgs : EventArgs
    {
        public ThumbnailControllerEventArgs(string imageFilename, int layout, string desc,  string title)
        {
            this.ImageFilename = imageFilename;
            this.layout = layout;
            this.description = desc;
            this.title = title;
        }

        public string ImageFilename;
        public int layout;
        public string description;
        public string title;
    }

    public delegate void ThumbnailControllerEventHandler(object sender, ThumbnailControllerEventArgs e);

    public class ThumbnailController
    {
        private bool m_CancelScanning;
        static readonly object cancelScanningLock = new object();

        public bool CancelScanning
        {
            get
            {
                lock (cancelScanningLock)
                {
                    return m_CancelScanning;
                }
            }
            set
            {
                lock (cancelScanningLock)
                {
                    m_CancelScanning = value;
                }
            }
        }

        public event ThumbnailControllerEventHandler OnStart;
        public event ThumbnailControllerEventHandler OnAdd;
        public event ThumbnailControllerEventHandler OnEnd;

        public ThumbnailController()
        {
            
        }

        public void AddFolder(int testId)
        {
            try
            {
               

                Thread thread = new Thread(new ParameterizedThreadStart(AddFolder));
                thread.IsBackground = true;
                thread.Start(testId);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        private void AddFolder(object tid)
        {
            try
            {
                int testId = (int)tid;

                if (this.OnStart != null)
                {
                    this.OnStart(this, new ThumbnailControllerEventArgs(null, 0, null, null));
                }

                this.PicturesIntern(testId);

                if (this.OnEnd != null)
                {
                    this.OnEnd(this, new ThumbnailControllerEventArgs(null, 0, null, null));
                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }

        }

        private void PicturesIntern(int testId)
        {

            try
            {
                List<PictureInfo> pb = MySQLConnector.GetPictures(testId, true);
                foreach (PictureInfo p in pb)
                {
                    Image img = null;

                    try
                    {
                        img = Image.FromFile(p.fullname);
                    }
                    catch
                    {
                        // do nothing
                    }

                    if (img != null)
                    {
                        this.OnAdd(this, new ThumbnailControllerEventArgs(p.fullname, 0, p.description, p.title));

                        img.Dispose();
                    }
                }
            }
            catch (Exception err)
            {
                throw (new SystemException("Picture before loading error: " + err.Message));
            }

            try
            {
                List<PictureInfo> pa = MySQLConnector.GetPictures(testId, false);
                foreach (PictureInfo p in pa)
                {
                    Image img = null;

                    try
                    {
                        img = Image.FromFile(p.fullname);
                    }
                    catch
                    {
                        // do nothing
                    }

                    if (img != null)
                    {
                        this.OnAdd(this, new ThumbnailControllerEventArgs(p.fullname, 1, p.description, p.title));

                        img.Dispose();
                    }
                }
            }
            catch (Exception err)
            {
                throw (new SystemException("Picture after loading error"));
            }
             
        }
    }
}
