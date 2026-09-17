using System;

namespace EPL_2024_standings
{
    public class StandingRecord
    {
        private string season = string.Empty;
        private int position;
        private int teamId;
        private string teamName = string.Empty;
        private int played;
        private int won;
        private int draw;
        private int lost;
        private int goals_for;
        private int goals_against;
        private int goal_diff;
        private int points;
        private string form = string.Empty;

        public StandingRecord() { }

        public string Season
        {
            get => season;
            set => season = value;
        }

        public int Position
        {
            get => position;
            set => position = value;
        }

        public int TeamId
        {
            get => teamId;
            set => teamId = value;
        }

        public string TeamName
        {
            get => teamName;
            set => teamName = value ?? string.Empty;
        }

        public int Played
        {
            get => played;
            set => played = value;
        }

        public int Won
        {
            get => won;
            set => won = value;
        }

        public int Draw
        {
            get => draw;
            set => draw = value;
        }

        public int Lost
        {
            get => lost;
            set => lost = value;
        }

        public int GoalsFor
        {
            get => goals_for;
            set => goals_for = value;
        }

        public int GoalsAgainst
        {
            get => goals_against;
            set => goals_against = value;
        }

        public int GoalDiff
        {
            get => goal_diff;
            set => goal_diff = value;
        }

        public int Points
        {
            get => points;
            set => points = value;
        }

        public string Form
        {
            get => form;
            set => form = value ?? string.Empty;
        }
    }
}