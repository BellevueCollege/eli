using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELI.Models
{
    /**
     * Basic model for Scores object
     * **/
    public class Scores
    {
        [Key]
        public int ScoreID { get; set; }
        public string Sid { get; set; }
        [RegularExpression(@"[1-5]\+?-?", ErrorMessage = "This score value must be in the range of 1- to 5+.")]
        public string WriteScore { get; set; }
        [Range(0,5)]
        public int? WritePlacement { get; set; }
        [RegularExpression(@"[1-5]\+?-?", ErrorMessage = "This score value must be in the range of 1- to 5+.")]
        public string OralScore { get; set; }
        [Range(0,5)]
        public int? OralPlacement { get; set; }
        [Range(0,100)]
        public int? EptScore { get; set; }
        [Range(0,5)]
        public int? EptPlacement { get; set; }
        public DateTime ModifyDt { get; set; }

        [Column("Username")]
        public string ModifyUsername { get; set; }

        public int? GetPlacementValueBasedOnEptScore()
        {
            if ( EptScore != null )
            {
                if (EptScore >= 0 && EptScore <= 29)
                {
                    return 1;
                }
                else if (EptScore >= 30 && EptScore <= 47)
                {
                    return 2;
                }
                else if (EptScore >= 48 && EptScore <= 65)
                {
                    return 3;
                }
                else if (EptScore >= 66 && EptScore <= 76)
                {
                    return 4;
                }
                else if (EptScore >= 77)
                {
                    return 5;
                }
            }
            return null;
        }
        public void AssignEptPlacement()
        {
            //if there's an EptScore and no placement, set the correct EptPlacement
            if (EptScore != null && EptPlacement == null )
            {
                EptPlacement = GetPlacementValueBasedOnEptScore();
            }
        }

        public void AssignOralPlacement()
        {
            if ( !String.IsNullOrEmpty(OralScore) && OralPlacement == null )
            {
                OralPlacement = Int32.Parse(OralScore.Substring(0, 1));
            }
        }

        public void AssignWritePlacement()
        {
            if ( WriteScore != null && WritePlacement == null)
            {
                WritePlacement = GetPlacementValueBasedOnEptScore();
            }
        }

        // custom comparator for comparing two scores for equality
        public bool IsEqualTo(Scores s)
        {
            if ( OralScore == s.OralScore && WriteScore == s.WriteScore && EptScore == s.EptScore 
                    && OralPlacement == s.OralPlacement && WritePlacement == s.WritePlacement && 
                    EptPlacement == s.EptPlacement )
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
