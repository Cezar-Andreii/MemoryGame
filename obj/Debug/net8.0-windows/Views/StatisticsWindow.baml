   M S B A M L   `   `   `  ����  Memoryy$    Memoryy.Views.StatisticsWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  
Statistici��5   	   $	��400��5   	   $	��600��5   	    � WindowStartupLocation$ CenterOwner=�5   	   .��5      � $��20q�6    � RowDefinitions 5	   
   �� 5
      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��Statistici utilizatori��6   $��24(�5      $
��Bold"�5      $��0,0,0,20q�5      &   System.Windows.Controls.DataGrid  5   
   $��1��6      AutoGenerateColumns$ False��5         
IsReadOnly$
 True��5         GridLinesVisibility$	 All=�5      ��5      �� *
Statistics+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Nume Utilizator��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 5   %   1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 5   %   �� *
Username+5   %     5      $ Jocuri Jucate��6%   $ * 5   %   	 5   %   �� *GamesPlayed+5   %     5      $ Jocuri Castigate��6%   $ * 5!   %   	 5    %   �� *
GamesWon+5!   %   5#      5$      5%      