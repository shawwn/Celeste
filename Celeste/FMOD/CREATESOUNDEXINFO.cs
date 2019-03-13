﻿// Decompiled with JetBrains decompiler
// Type: FMOD.CREATESOUNDEXINFO
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD
{
  public struct CREATESOUNDEXINFO
  {
    public int cbsize;
    public uint length;
    public uint fileoffset;
    public int numchannels;
    public int defaultfrequency;
    public SOUND_FORMAT format;
    public uint decodebuffersize;
    public int initialsubsound;
    public int numsubsounds;
    public IntPtr inclusionlist;
    public int inclusionlistnum;
    public SOUND_PCMREADCALLBACK pcmreadcallback;
    public SOUND_PCMSETPOSCALLBACK pcmsetposcallback;
    public SOUND_NONBLOCKCALLBACK nonblockcallback;
    public IntPtr dlsname;
    public IntPtr encryptionkey;
    public int maxpolyphony;
    public IntPtr userdata;
    public SOUND_TYPE suggestedsoundtype;
    public FILE_OPENCALLBACK fileuseropen;
    public FILE_CLOSECALLBACK fileuserclose;
    public FILE_READCALLBACK fileuserread;
    public FILE_SEEKCALLBACK fileuserseek;
    public FILE_ASYNCREADCALLBACK fileuserasyncread;
    public FILE_ASYNCCANCELCALLBACK fileuserasynccancel;
    public IntPtr fileuserdata;
    public int filebuffersize;
    public CHANNELORDER channelorder;
    public CHANNELMASK channelmask;
    public IntPtr initialsoundgroup;
    public uint initialseekposition;
    public TIMEUNIT initialseekpostype;
    public int ignoresetfilesystem;
    public uint audioqueuepolicy;
    public uint minmidigranularity;
    public int nonblockthreadid;
    public IntPtr fsbguid;
  }
}
