   M S B A M L   `   `   `  ����@  <Paint, Version=0.0.0.1, Culture=neutral, PublicKeyToken=nullclr-namespace:PaintPaint  5   	   R NColorPicker, Version=3.3.1.0, Culture=neutral, PublicKeyToken=1c61eec504ce2276>.clr-namespace:ColorPicker;assembly=ColorPickerColorPicker 5   	   3clr-namespace:Paint.PropertiesPaint.Properties  5   	       Paint.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:Paint     5   	   Fcolorpicker.clr-namespace:ColorPicker;assembly=ColorPicker     5   	   5
Propertiesclr-namespace:Paint.Properties     5   	   -   5          � Title$  Sigma Paint��5   	   $	��450��6   $	��800��6*    � WindowStartupLocation$ CenterScreen=�6S    � Icon$&  /Paint;component/Images/icon.ico��6�   ��5      �� 5   
    �� ImageSource$ ../Images/Cheker.jpg��6    j� TileMode$
 Tile=�68    j� ViewportUnits$ Absolute=�6H    j� Viewport$ 	0,0,32,32�6a   .��5      � .+�5   
   o� -   5         o�Name$ MainMenu��5      $��18��6!   $	��Top=�6-   ���	#FF333333 K� ZIndex$ 4��6\   $��0,0,0.333,0q�6m   .@�5      m� $
��File��6   $��18��6%   $��37��61   
�������.@�5      m� -   5        	 m� InputGestureText$	 Ctrl+O��5      $��Open..��66   ���	#FF0A0A0A6]   m� 5      -   5        $	 Ctrl+N��5      $��Create new..��65   ���	#FF0A0A0A6b   m� 5      -   5        $	 Ctrl+S��5      $��	Save as..��65   ���	#FF0A0A0A6_   �� 5      $��0q�6   m� 5      -   5        $	 Alt+F4��5      $��Close��65   ���	#FF0A0A0A6[   5      m� 5      $
��Edit��6   
�������.@�5      m� -   5        $	 Ctrl+Z��5      $
��Undo��65   ���	#FF0A0A0A6Z   m� 5      -   5        $	 Ctrl+Shift+Z��5      $
��Redo��6;   ���	#FF0A0A0A6`   5      m� 5      $��Addons��6   
�������.@�5      m� -	   5        $��Tools��5      ���	#FF0A0A0A6A   m� 5      -
   5        $��Plugins��5      ���	#FF0A0A0A6C   m� 5       -   5        $	 Ctrl+P��5       $��
All Addons��66   ���	#FF0A0A0A6a   5!      m� 5"      
�������$
��Help��6*   .@�5#      m� -   5        ���	#FF0A0A0A$��Creator��5#   2   6C   m� 5$      -   5        $	 .NET Framework 4.5.1��5$      ���	#FF0A0A0A$��Sources��6[   6l   5%      5&      �� 5'   
   -   5        
 ��Name$
 UpShadow��5'      
���   �$��1q�67   $��126,0,0.333,0q�6K   $	��Top=�6b   $ 1��6z   $��18��6�    J� Effect 5(      3  -System.Windows.Media.Effects.DropShadowEffect  5)         ShadowDepth$ 2Q�6#   5+      � 5,   
   -   5         �Name$ 	GridFrame��5,      $��126,18,0.333,-0.333q�6"   $��2��6�   ���	#00000000.+�5/      �� -   5         ��Name$ 	MainImage��5/      $	��204��6'   $	��280��64    J� RenderTransformOrigin$ 0.5,0.5&�6@   $��0q�6`   $
��Left=�6k   $	��Top=�6�   $
��Fill=�6�    �� BitmapScalingMode$ NearestNeighbor=�6�    50        51      $ 0Q�6'   53      �� 54      -   5        $ 
BrushImage��54      $ NearestNeighbor=�6(   $
��Left=�6Z   $	��128��6u   $��0q�6�   $	��Top=�6�   $	��128��6�   $(��"/Paint;component/Images/Brush1.png��6�   �� 55      -   5        $ 	FillImage��55      $ NearestNeighbor=�6'   $
��Left=�6Y   $��49��6t   $��
201,49,0,0q�6�   $	��Top=�6�   $��49��6�   $,��&/Paint;component/Images/FillCursor.png��6�   56      �� 5D   
   -   5         ��Name$ 	LeftPanel��5D      $��0,18,0,-0.333q�6*   ���	#FF1E1E1E$
��Left=�6X   $	��126��6s   $��Hidden=�6   .��5E      �� -   5         ��Name$ LeftPanelHolder��5E      $
��Left=�62   $	��126��6M   $ 1��6Y    �� CanVerticallyScroll . ��.  � UseLayoutRounding .  $��Hidden=�6�   $	��403��6�   $	��Top=�6   5F        5G      $ 1Q�6'   .+�5I      �� $	��241��6   .+�5J      %  ColorPicker.StandardColorPicker  -   5          Name$ ColorPicker��5J   6   $	��224��6K   $��	0,0,-74,0q�6X   $ 0,0.5&�6k      
PickerType$	 HSL=�6�   #  ColorPicker.PickerControlBase   SelectedColor$ White��6�   '  !ColorPicker.DualPickerControlBase   UseHintColor . ���	#FFE6E6E6���	#FF1E1E1Ez�5K      V� 5L      .~�5M   "   ��  �� ScaleX$
 0.63Q�61    �� ScaleY$
 0.63Q�6?   �� 5N   "   �� 5O   "   T� 5P   "    T� Y$	 -70Q�65   5Q      5S      5T      �� 5U      $��50��6   .+�5V      � $	��100��6   .+�5W      �� ���	#FFACACAC$��0.5,0.5,0,0.5q�69   
�������$	��6,0q�6l   $	��Top=�6y   $��46��6�   �� 5X      -   5         ��Name$ BrushSizeLabel��5X       $��
Brush Size��68   $	��6,0q�6M   $	��Top=�6Z   $��Center=�6r   $��9(�6�   $��19��6�   $��5,3,5,5q�6�   �� 5Y      -   5         ��Name$ BrushSizeSlider��5Y   !   $��Center=�6:   $	��Top=�6W   $	��106��6o   $��
10,19,10,0q�6{    � Maximum$ 4Q�6�    � Minimum$	 0.1Q�6�     � LargeChange$	  0.1Q�6�   ! � Value$! 1Q�6  5Z      5[      �� 5\      $��61��6   .+�5]      � $	��100��6   .+�5^      �� ���	#FFACACAC$��0.5,0.5,0,0.5q�69   
�������$	��6,0q�6l   $	��Top=�6y   $��22��6�   �� 5_      -   5        $ BrushTypeText��5_       $��
Brush Type��67   $	��6,0q�6L   $	��Top=�6Y   $��Center=�6q   $��9(�6�   �� 5`      -   5        " ��Name$" SelectBrushButton��5`   !   $��Select��6<   $��
6,27,76,56q�6M   ���	#FFE6E6E6���	#FFF2F2F26�   �� 5a      -   5        $ BrushPreview��5a       $
��Left=�66   $��53��6Q   $	��Top=�6]   $��52��6u   $(��"/Paint;component/Images/Brush1.png��6�   $��64,5,0,0q�6�    5b        5c   "   $ 0Q�63   5e      5f      5g      �� 5h      $��50��6   .+�5i      � $
��Left=�6   $��50��66   $	��Top=�6B   $	��126��6Z   .+�5j      �� -   5        $" 
FillButton��5j   !   $��	7,5,46,28q�65   
�������$
��Fill��6[   $ 0.5,0.5&�6�   z�5k      V� 5l   "   .~�5m   &   �� �� 5n   &   # �� AngleY$# -0.296Q�64   �� 5o   &   T� 5p   &   $ -0.217Q�69   5q   #   5s      �� 5t      -   5        $ FillIcon��5t       $��	90,0,8,22q�62   $&�� /Paint;component/Images/Fill.png��6E   $ J� Opacity$	$ 0.2Q�6o   �� 5u      -   5        $ 	FillDelta��5u   !   $��Center=�64   $	��Top=�6Q   $��76��6i   $��	7,28,43,0q�6t   $	 254Q�6�   % � SmallChange$% 1Q�6�   $! 1Q�6�   $ 1Q�6  �� 5v      -   5        $ FillDeltaText��5v       $��1��67   $
��Left=�6C   $��	87,23,0,0q�6^   $	��Top=�6q   
�������$��33��6�   $��Center=�6�   $��Center=�6�   $��23��6�   {�.  6�   5w      5x      5y      5z      5|      5}      