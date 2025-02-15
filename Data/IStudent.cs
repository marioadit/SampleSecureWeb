﻿using System;
using SampleSecureWeb.Models;

namespace SampleSecureWeb;

public interface IStudent
{
    IEnumerable<Student> GetStudents();
    Student GetStudent(string Nim);
    Student AddStudent(Student student);
    Student UpdateStudent(Student student);
    void DeleteStudent(string Nim);
}
