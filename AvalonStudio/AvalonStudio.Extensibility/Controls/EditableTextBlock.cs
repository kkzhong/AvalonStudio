﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Threading;
using System;

namespace AvalonStudio.Controls
{
    public class EditableTextBlock : TemplatedControl
    {
        private string _text;
        private string _editText;
        private TextBox _textBox;
        private bool _focusedOnClick1 = false;
        private DispatcherTimer _editClickTimer;


        public EditableTextBlock()
        {
            _editClickTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500),                
            };

            _editClickTimer.Tick += (sender, e) =>
             {
                 _editClickTimer.Stop();

                 if(IsFocused && !InEditMode)
                 {
                     EnterEditMode();
                 }
             };

            this.GetObservable(TextProperty).Subscribe(t =>
            {
                EditText = t;
            });

            this.GetObservable(InEditModeProperty).Subscribe(mode =>
            {
                if (mode && _textBox != null)
                {
                    EnterEditMode();
                }
            });

            AddHandler(PointerPressedEvent, (sender, e)=>
            {
                _editClickTimer.Stop();

                if (!InEditMode)
                {                    
                    if (e.ClickCount == 1)
                    {
                        if(IsFocused)
                        {
                            _editClickTimer.Start();
                        }
                    }                    
                }
                else
                {
                    var hit = this.InputHitTest(e.GetPosition(this));

                    if (hit == null)
                    {
                        ExitEditMode();
                    }
                }
            }, RoutingStrategies.Tunnel);
        }

        public static readonly DirectProperty<EditableTextBlock, string> TextProperty = TextBlock.TextProperty.AddOwner<EditableTextBlock>(
                o => o.Text,
                (o, v) => o.Text = v,
                defaultBindingMode: BindingMode.TwoWay,
                enableDataValidation: true);

        [Content]
        public string Text
        {
            get { return _text; }
            set
            {
                SetAndRaise(TextProperty, ref _text, value);
            }
        }

        public string EditText
        {
            get => _editText;
            set
            {
                SetAndRaise(EditTextProperty, ref _editText, value);
            }
        }

        public static readonly DirectProperty<EditableTextBlock, string> EditTextProperty =
                AvaloniaProperty.RegisterDirect<EditableTextBlock, string>(nameof(EditText), o => o.EditText, (o, v) => o.EditText = v);

        public static readonly StyledProperty<bool> InEditModeProperty =
            AvaloniaProperty.Register<EditableTextBlock, bool>(nameof(InEditMode), defaultBindingMode: BindingMode.TwoWay);

        public bool InEditMode
        {
            get { return GetValue(InEditModeProperty); }
            set { SetValue(InEditModeProperty, value); }
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);

            _textBox = e.NameScope.Find<TextBox>("PART_TextBox");

            if (InEditMode)
            {
                EnterEditMode();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                ExitEditMode();
            }            

            base.OnKeyUp(e);
        }

        private void EnterEditMode()
        {
            EditText = Text;
            InEditMode = true;
            (VisualRoot as IInputRoot).MouseDevice.Capture(_textBox);
            _textBox.CaretIndex = Text.Length;
            _textBox.SelectionStart = 0;
            _textBox.SelectionEnd = Text.Length;

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                _textBox.Focus();
            });
        }

        private void ExitEditMode()
        {
            Text = EditText;

            InEditMode = false;
            (VisualRoot as IInputRoot).MouseDevice.Capture(null);
        }        
    }
}
