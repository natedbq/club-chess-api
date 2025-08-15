using chess.api.models;
using chess.api.repository;
using ChessApi.repository;

namespace chess.api.services
{
    public class MigrationUCI
    {
        private readonly StudyRepository studyRepo;
        private readonly SimpleStudyRepository simpleStudyRepo;
        private readonly PositionRepository positionRepo;
        public MigrationUCI()
        {
            studyRepo = new StudyRepository();
            simpleStudyRepo = new SimpleStudyRepository();
            positionRepo = new PositionRepository();
        }

        public void Execute()
        {
            //var studies = simpleStudyRepo.GetStudies();
            //foreach(var simpleStudy in studies)
            //{
            //    var study = studyRepo.GetStudyById(simpleStudy.Id);
            //    var topPosition = positionRepo.GetById(study.Position.Id, depth:200);

            //    InferUCI(topPosition);
            //}
        }

        private void InferUCI(Position p1)
        {

            var b1 = FenToBoard(p1.Move.FEN);

            foreach(var p2 in p1.Positions)
            {
                if(p2.Move.From == null || p2.Move.From.Trim() == "")
                {
                    var diffs = new List<DiffItem>();

                    var b2 = FenToBoard(p2.Move.FEN);
                    for (int row = 0; row < 8; row++)
                    {
                        for (int col = 0; col < 8; col++)
                        {
                            var s1 = b1[row, col];
                            var s2 = b2[row, col];
                            if (s1 != s2)
                            {
                                diffs.Add(new DiffItem()
                                {
                                    location = "abcdefgh"[col] + (row + 1).ToString(),
                                    b1 = s1,
                                    b2 = s2
                                });
                            }
                        }
                    }
                    var uci = new UCI();

                    if (diffs.Count == 2) //standard move
                    {
                        if (diffs[0].b2 == "o")
                        {
                            uci.from = diffs[0].location;
                            uci.to = diffs[1].location;
                        }
                        else
                        {
                            uci.from = diffs[1].location;
                            uci.to = diffs[0].location;
                        }
                    }
                    else { 
                        if(diffs.Select(d => d.location[1]).Distinct().Count() > 1) // en Passant
                        {
                            var landingSpace = diffs.FirstOrDefault(x => x.b2 != "o");
                            var fromSpace = diffs.FirstOrDefault(x => x.location[0] != landingSpace.location[0]);
                        }else //castles
                        {
                            var rank = diffs.First().location[1];
                            var rookColumn = diffs.First(d => d.location[0].ToString() == "h" || d.location[0].ToString() == "a").location[0].ToString();
                            if(rookColumn == "h")
                            {
                                uci.from = "e" + rank;
                                uci.to = "g" + rank;
                            }
                            else
                            {
                                uci.from = "e" + rank;
                                uci.to = "c" + rank;
                            }
                        }
                    }

                    p2.Move.From = uci.from;
                    p2.Move.To = uci.to;
                    p2.Tags.Add("uci_mig_3");
                    //positionRepo.Save(p2);
                }
                InferUCI(p2);
            }

        }

        private string[,] FenToBoard(string fen)
        {
            fen = fen.Substring(0, fen.IndexOf(" "));
            var board = new string[8,8];

            int row = 7;
            int col = 0;

            foreach(var f in fen)
            {
                if(f == '/')
                {
                    row--;
                    col = 0;
                    continue;
                }
                int x;
                if(int.TryParse(f.ToString(), out x))
                {
                    for(int i = 0; i < x; i++)
                    {
                        board[row, col] = "o";
                        col++;
                    }
                }
                else
                {
                    board[row, col] = f.ToString();
                    col++;
                }
            }
            return board;
        }

        private class UCI
        {
            public string from { get; set; }
            public string to { get; set; }
        }

        private class DiffItem
        {
            public string location { get; set; }
            public string b1 { get; set; }
            public string b2 { get; set; }
        }
    }

}
