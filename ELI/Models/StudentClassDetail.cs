﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELI.Models
{
    public class StudentClassDetail
    {
        public string Sid { get; private set; }
        public string YearQuarterID { get; private set; }
        public string ItemNumber { get; private set; }
        public string GradeID { get; private set; }
        public string InstructorName { get; private set; }
        public string CourseTitle { get; private set; }
        public string Country { get; private set; }
    }
}