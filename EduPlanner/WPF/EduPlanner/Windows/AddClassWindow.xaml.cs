﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EduPlanner {
    /// <summary>
    /// Interaction logic for AddClass.xaml
    /// </summary>
    public partial class AddClassWindow : Window {

        List<ClassTime> classTimes = new List<ClassTime>();

        public AddClassWindow() {
            InitializeComponent();
        }

        #region Event Handlers

        private void AddTime_Click(object sender, RoutedEventArgs e) {
            ClassTime time = new ClassTime();
            time.ClassTimeChanged += new ClassTime.ClassTimeDelegate(Handler);
            classTimes.Add(time);
            spClassTimesViewer.Children.Add(time);
            Handler();
        }

        public void AddClass_Click(object sender, RoutedEventArgs e) {

            //Loop through class times
            for (int i = 0; i < spClassTimesViewer.Children.Count; i++) {
                ClassTime time = spClassTimesViewer.Children[i] as ClassTime;

                WrapPanel panel = time.FindName("wpDays") as WrapPanel;
                Grid timeGrid = time.FindName("TimeGrid") as Grid;

                //Loop through checkboxes (days)
                for (int j = 0; j < panel.Children.Count; j++) {

                    //Check the current checkbox
                    if (panel.Children[j] is CheckBox) {
                        CheckBox check = panel.Children[j] as CheckBox;
                        if (check.IsChecked == true) {

                            //Get day as an enum
                            Enum.TryParse(check.Name, out DayOfWeek day);

                            DateTime? startTime = (timeGrid.Children[0] as TimePicker).SelectedTime;
                            DateTime? endTime = (timeGrid.Children[1] as TimePicker).SelectedTime;

                            //Check if this class already exists
                            Class newClass = CheckClassExistence(txtClassName.Text);
                            if (newClass == null) {
                                //Creates a new one if not
                                newClass = new Class(txtClassName.Text);
                                DataManager.Schedule.classes.Add(newClass);
                            }

                            //Changes the old one
                            newClass.classTimes[(int)day][0] = startTime;
                            newClass.classTimes[(int)day][1] = endTime;

                            DataManager.Schedule.days[(int)day].classes.Add(newClass);
                            DataManager.Schedule.days[(int)day].hasClass = true;
                        }
                    }
                }
            }

            Close();
        }

        private void ClassName_TextChanged(object sender, TextChangedEventArgs e) {
            Handler();
        }

        #endregion

        private Class CheckClassExistence(string name) {
            if (DataManager.Schedule.classes != null) {
                for (int i = 0; i < DataManager.Schedule.classes.Count; i++) {
                    if (DataManager.Schedule.classes[i].className == name)
                        return DataManager.Schedule.classes[i];
                }
            }

            return null;
        }

        private void Handler() {
            bool dayChecked = false;
            bool timePicked = false;

            //Loop through class times
            for (int i = 0; i < spClassTimesViewer.Children.Count; i++) {
                ClassTime time = spClassTimesViewer.Children[i] as ClassTime;

                WrapPanel panel = time.FindName("wpDays") as WrapPanel;
                Grid timeGrid = time.FindName("TimeGrid") as Grid;

                //Loop through checkboxes
                for (int j = 0; j < panel.Children.Count; j++) {

                    if (panel.Children[i] is CheckBox) {
                        CheckBox check = panel.Children[j] as CheckBox;
                        if (check.IsChecked == true)
                            dayChecked = true;
                    }
                }

                //Loop through time pickers
                for (int j = 0; j < timeGrid.Children.Count; j++) {
                    if (timeGrid.Children[j] is TimePicker) {
                        TimePicker picker = timeGrid.Children[j] as TimePicker;
                        timePicked = picker.SelectedTime != null;
                    }
                }
            }

            if (dayChecked && timePicked && txtClassName.Text != "")
                btnAddClass.IsEnabled = true;
            else
                btnAddClass.IsEnabled = false;
        }
    }
}