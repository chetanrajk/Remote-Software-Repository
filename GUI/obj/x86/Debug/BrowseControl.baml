   M S B A M L   `   `   `  ����B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5          WpfApp1.BrowseControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      'localclr-namespace:WpfApp1     5      .��5      �   � RowDefinitions  5   
   �� 5      $	��240 �6   6+   �� 5      $��3 �6   6)   �� 5      $��* �6   6)   .+�5   
   � $��0��6   ���	#FFF5F5F5  5      �� 5      $��30 �6    6.   �� 5      $��35 �6    6.   �� 5      $��* �6    6-   .+�5"      ]� $��0��6   .+�5#      �� $
��Left=�6   $��10,6.55,10,5q�62   $��Center=�6H   $��14(�6c   $
��Bold"�6q   $��18��6�   $��135.5��6�   $
��Left=�6�   $��Remote Directories��6�   �� 5$      $
��Left=�6   $��250,10,10,5q�62   $��Center=�6G   $��14(�6b   $
��Bold"�6p   $��18��6�   $	��234��6�   $��Files ��6�   $��Center=�6�   5%      ]� 5&      $��1��6   .+�5'      �� -   5         ��Name$ PathTextBlock��5'      $
��Left=�63    �� Padding$
 10 0q�6I   $��
12,7,12,10q�6X   $��Center=�6l   $��14(�6�   $��Normal"�6�   ���#fff$��18��6�   $	��310��6�   $
��Left=�6�   path6�   �� 5(      $
��Left=�6   $��130,10,10,5q�62   $��Center=�6G   $��12(�6b   $
��Bold"�6p   $��18��6�   $	��294��6�   $6��0Double click on a file to view text and metadata��6�   $��Center=�6�   5)      �� 5*      $��2��6   $��10q�6"   ���#000$��	0,0,470,0q�6I   ._�5+      �� -   5         ��Name$ DirList��5+      6+   5,      �� 5-      $��2��6   $��10q�6"   ���#000$��	470,0,0,0q�6I   ._�5.      �� -   5        $ FileList��5.      6[   5/      50      �� 51   
   $��1��6   $��3��6$   $��3��68   $��Stretch=�6C   ���#6666u   � 52   
   $��2��6   ���	#FFF5F5F5  53      �� 54      $��30 �6    6.   �� 56      $��* �6    6-   �� 58      $��30 �6    6.   .+�5;      �� $��0��6   $ 10 10q�6%   $
��Bold"�65   $��14(�6G   $%��Browse files by specific filter��6U   �� 5<      $��File List Result: ��6   $
��Left=�61   $��
498,10,0,0q�6L   $��1��6`   $	��Top=�6m   $��96.96��6�   $
��Bold"�6�   �� 5=      -   5        $��FileListRes��5=      $
��Left=�6Y   $	��228��6t   $��498,50.5,0,0q�6�   $��1��6�   $	��Top=�6�   $	��292��6�   �� 5>      $��
File Name:��6   $
��Left=�6)   $��10,32.55,0,0q�6D   $��1��6Z   $	��Top=�6g   $��66.993��6   �� 5?      $��Categories:��6   $
��Left=�6*   $��10,79.55,0,0q�6E   $��1��6[   $	��Top=�6h   $��66.993��6�   �� 5@      $��Dependency:��6   $
��Left=�6*   $��10,172.55,0,0q�6E   $��1��6\   $	��Top=�6i   $��79.993��6�   �� 5A      $��Version:��6   $
��Left=�6'   $��10,216.55,0,0q�6B   $��1��6Y   $	��Top=�6f   $��66.993��6~   �� 5B      -   5        $��txtFname��5B      $
��Left=�6&   $��23��6A   $��102,32.55,0,0q�6M   $��1��6d    �� TextWrapping$
 Wrap=�6q   $	��Top=�6�   $	��177��6�   �� 5C      -   5        $��txtDep��5C      $
��Left=�6$   $��23��6?   $��102,172.55,0,0q�6K   $��1��6c   $
 Wrap=�6p   $	��Top=�6�   $	��177��6�   �� 5D      -   5        $��txtVer��5D      $
��Left=�6$   $��23��6?   $��102,216.55,0,0q�6K   $��1��6c   $
 Wrap=�6p   $	��Top=�6�   $	��177��6�    J� RenderTransformOrigin$ 0.514,-1.154&�6�   �� 5E      -   5        $��qCatList��5E      $
��Left=�6&   $��89��6A   $��102,67.55,0,0q�6M   $��1��6d   $	��Top=�6q   $	��177��6�   .@�5F      �� -	   5         ��Name$ SrcFile��5F      .��6/   Source File6<   �� 5G      -
   5        $ HeadFile��5G      .��60   Header File6=   �� 5H      -   5        $ CommFea��5H      .��6/   Comm Feature6=   �� 5I      -   5        $
 Func��5I      .��6,   Functionality6;   �� 5J      -   5        $ MsgFea��5J      .��6.   Message Feature6?   5K      �� 5L      -   5        $��Browse  >>>��5L   0   $
��Left=�6O   $��346,119.5,0,0q�6j   $��1��6�   $	��Top=�6�   $	��103��6�   $��37.05��6�   �� 5M      $��
No Parent:��6   $
��Left=�6)   $��10,260.549,0,0q�6D   $��1��6\   $	��Top=�6i   $��66.993��6�   �� 5N      -   5        $��
qChkParent��5N      $�� ��6)   $
��Left=�64   $��102,267,0,0q�6O   $��1��6d   $	��Top=�6q   $ 
0.3,-1.229&�6�    d� 	IsChecked$ FalseY�6�   5R      5S      5T      