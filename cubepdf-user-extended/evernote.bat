@echo off
:: ------------------------------------------------------------------------- ::
::
::  evernote.bat
::
::  Copyright (c) 2010 CubeSoft. All rights reserved.
::
::  :: �Ŏn�܂�s�̓R�����g�Ƃ��ė��p���Ă��܂��D
::
::  ''����''
::  ���̃X�N���v�g���g�p���邽�߂ɂ́C���s���� Evernote for Windows
::  ���C���X�g�[������Ă���K�v������܂��D
::  Evernote for Windows �͈ȉ��� URL ����_�E�����[�h���ĉ������D
::  http://www.evernote.com/about/download/windows.php
::
::  Last-update: Mon 26 Jul 2010 10:05:00 JST
::
:: ------------------------------------------------------------------------- ::

:: ------------------------------------------------------------------------- ::
::
::  Evernote �ɃA�b�v���[�h���邽�߂̐ݒ�
::
::  evernote: Evernote �̃C���X�g�[���t�H���_���w�肷��
::  username: �ȗ����͍ŏI���O�C�����[�U
::  password: username �ȗ����͎g�p���Ȃ�
::
:: ------------------------------------------------------------------------- ::
set evernote="C:\Program Files\Evernote\Evernote3.5\ENScript.exe"
set username=""
set password=""

:: ------------------------------------------------------------------------- ::
::
::  ���ۂ̏���
::
::  �f�t�H���g�ł͎��s���邽�тɃT�[�o�Ɠ������Ă���D
::  �蓮�œ�������ꍇ�́CsyncDatabase �̍s���R�����g�A�E�g����D
::
:: ------------------------------------------------------------------------- ::
if not exist %1 (
    echo %1: �t�@�C����������܂���ł���
    exit /b
)

echo %evernote% createNote /s %1
echo %evernote% syncDatabase

if %username% == "" (
    %evernote% createNote /s %1
    %evernote% syncDatabase
) else (
    %evernote% createNote /s %1 /u %username% /p %password%
    %evernote% syncDatabase /u %username% /p %password%
)

exit /b
