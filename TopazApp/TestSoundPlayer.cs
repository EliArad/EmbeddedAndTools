using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TopazApp
{
    public class TestSoundPlayer
    {
        System.Media.SoundPlayer player = null;

        public enum SOUND_TYPES
        {
            NO_SOUND,
            STOP_SOUND,
            FATAL_ERROR,
            END_OF_COOKING,
            CHANGE_LINE

        }
        public void StopSound()
        {
            if (player != null)
                player.Stop();
        }

        public void playSound(SOUND_TYPES type, bool loop = false)
        {
            try
            {
                if (type == SOUND_TYPES.NO_SOUND)
                    return;
                else
                    if (type == SOUND_TYPES.STOP_SOUND)
                    {
                        StopSound();
                        return;
                    }
                    else
                        if (type == SOUND_TYPES.FATAL_ERROR)
                        {
                            while (player != null)
                            {
                                Thread.Sleep(100);
                            }
                            player = new System.Media.SoundPlayer(@"c:\Windows\Media\Windows Notify.wav");
                            player.Play();
                            player = null;
                            return;
                        }
                        else
                            if (type == SOUND_TYPES.END_OF_COOKING)
                            {
                                while (player != null)
                                {
                                    Thread.Sleep(100);
                                }
                                player = new System.Media.SoundPlayer(@"c:\Windows\Media\Windows Shutdown.wav");
                                player.Play(); // can also use soundPlayer.PlaySync()
                                player = null;
                                return;
                            }
                            else
                                if (type == SOUND_TYPES.CHANGE_LINE)
                                {
                                    while (player != null)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    player = new System.Media.SoundPlayer(@"c:\Windows\Media\Windows Shutdown.wav");

                                    if (loop == false)
                                        player.Play(); // can also use soundPlayer.PlaySync()
                                    else
                                        player.PlayLooping();
                                    player = null;
                                    return;
                                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }

        }
    }
}
