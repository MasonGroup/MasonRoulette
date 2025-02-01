using System;
using System.IO;
using System.Media;
using System.Threading;

namespace MasonGame
{
    class BB2
  {
    private const int SampleRate = 9489;
    private const int DurationSeconds = 10;
    private const int BufferSize = SampleRate * DurationSeconds;

    private static Func<int, int>[] formulas = new Func<int, int>[] {
        t => 2*t&t>>8|5*t&t>>7|9*t&t>>4|15*t&t>>4
    };
    public static Func<int, int>[] Formulas { get => formulas; set => formulas = value; }
    private static byte[] GenerateBuffer(Func<int, int> formula)
    {
        byte[] buffer = new byte[BufferSize];
        for (int t = 0; t < BufferSize; t++)
        {
            buffer[t] = (byte)((formula(t) & 0xFF) / 17);
        }
        return buffer;
    }

    private static void SaveWav(byte[] buffer, string filePath)
    {
        using (var fs = new FileStream(filePath, FileMode.Create))
        using (var bw = new BinaryWriter(fs))
        {
            bw.Write(new[] { 'R', 'I', 'F', 'F' });
            bw.Write(36 + buffer.Length);
            bw.Write(new[] { 'W', 'A', 'V', 'E' });
            bw.Write(new[] { 'f', 'm', 't', ' ' });
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)1);
            bw.Write(SampleRate);
            bw.Write(SampleRate);
            bw.Write((short)1);
            bw.Write((short)8);
            bw.Write(new[] { 'd', 'a', 't', 'a' });
            bw.Write(buffer.Length);
            bw.Write(buffer);
        }
    }
    private static void PlayBufferLoop(byte[] buffer)
    {
        string tempFilePath = Path.GetTempFileName();
        SaveWav(buffer, tempFilePath);

        using (SoundPlayer player = new SoundPlayer(tempFilePath))
        {
            player.PlayLooping();
            Thread.Sleep(Timeout.Infinite);
        }
        File.Delete(tempFilePath);
    }
    public static void PlayBytebeatAudioLoop()
    {
        foreach (var formula in Formulas)
        {
            byte[] buffer = GenerateBuffer(formula);
            PlayBufferLoop(buffer);
      }
    }
  }
}