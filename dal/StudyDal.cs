using chess.api.models;

namespace ChessApi.dal
{
    public class StudyDal
    {
        public IList<Study> GetStudies()
        {
            return _studyList.ToList();
        }

        public Study GetById(Guid id)
        {
            return _studyList.ToList().FirstOrDefault(s => s.Id == id);
        }

        private static IList<Study> _studyList = new List<Study>()
            {
                new Study()
                {
                    Id = Guid.NewGuid(),
                    Title = "New Study",
                    Description = "Begin a new study",
                    Position = new Position()
                    {
                        Id= Guid.NewGuid(),
                        Title = "",
                        Tags = new List<string>(),
                        Description = "",
                        Move = new Move("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1","-"),
                        Positions = new List<Position>()
                        {
                            new Position()
                            {
                                Id= Guid.NewGuid(),
                                Title = "",
                                Tags = new List<string>(),
                                Description = "",
                                Move = new Move("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1","e4"),
                                Positions = new List<Position>()
                                {
                                    new Position(){
                                        Id= Guid.NewGuid(),
                                        Title = "",
                                        Tags = new List<string>(),
                                        Description = "",
                                        Move = new Move("rnbqkbnr/pp1ppppp/2p5/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2","c6"),
                                        Positions = new List<Position>()
                                        {
                                            new Position(){
                                                Id= Guid.NewGuid(),
                                                Title = "",
                                                Tags = new List<string>(),
                                                Description = "",
                                                Move = new Move("rnbqkbnr/pp1ppppp/2p5/8/3PP3/8/PPP2PPP/RNBQKBNR b KQkq d3 0 2","d4"),
                                                Positions = new List<Position>()
                                                {
                                                    new Position(){
                                                        Id= Guid.NewGuid(),
                                                        Title = "",
                                                        Tags = new List<string>(),
                                                        Description = "",
                                                        Move = new Move("rnbqkbnr/pp2pppp/2p5/3p4/3PP3/8/PPP2PPP/RNBQKBNR w KQkq d6 0 3","d5"),
                                                        Positions = new List<Position>()
                                                        {
                                                            new Position(){
                                                                Id= Guid.NewGuid(),
                                                                Title = "",
                                                                Tags = new List<string>(),
                                                                Description = "",
                                                                Move = new Move("rnbqkbnr/pp2pppp/2p5/3pP3/3P4/8/PPP2PPP/RNBQKBNR b KQkq - 0 3","e5"),
                                                                Positions = new List<Position>()
                                                                {
                                                                    new Position(){
                                                                        Id= Guid.NewGuid(),
                                                                        Title = "",
                                                                        Tags = new List<string>(),
                                                                        Description = "",
                                                                        Move = new Move("rnbqkbnr/pp2pppp/8/2ppP3/3P4/8/PPP2PPP/RNBQKBNR b KQkq - 0 3","c5")
                                                                    }
                                                                }
                                                            },
                                                            new Position(){
                                                                Id= Guid.NewGuid(),
                                                                Title = "",
                                                                Tags = new List<string>(),
                                                                Description = "",
                                                                Move = new Move("rnbqkbnr/pp2pppp/2p5/3P4/3P4/8/PPP2PPP/RNBQKBNR b KQkq - 0 3","exd5"),
                                                                Positions = new List<Position>()
                                                                {
                                                                    new Position(){
                                                                        Id= Guid.NewGuid(),
                                                                        Title = "",
                                                                        Tags = new List<string>(),
                                                                        Description = "",
                                                                        Move = new Move("rnbqkbnr/pp2pppp/8/3p4/3P4/8/PPP2PPP/RNBQKBNR w KQkq - 0 4","cxd5")
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    SummaryFEN = "rnbqkbnr/pp1ppppp/2p5/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2",
                    Perspective = Color.Black
                },
                //new Study()
                //{
                //    Id = Guid.NewGuid(),
                //    Title = "Danish Gambit",
                //    Description = "Fun!",
                //    Continuation = new Continuation()
                //    {
                //        Id = Guid.NewGuid(),
                //        Title = "Danish Gambit",
                //        Description = "The best opening on the market.",
                //        MovesToPosition = new List<Move>()
                //        {

                //            new Move("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "-"),
                //            new Move("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", "e4"),
                //            new Move("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2", "e5")
                //        },
                //        Position = new Position()
                //        {
                //            Id = Guid.NewGuid(),
                //            Title = "Danish Gambit",
                //            Tags = new List<string>(){"Danish Gambit", "Gambit" },
                //            Description = "The opening position.",
                //            Move = new Move("rnbqkbnr/pppp1ppp/8/4p3/3PP3/8/PPP2PPP/RNBQKBNR b KQkq d3 0 2", "d4"),
                //            Continuations = new List<Continuation>() {
                //                new Continuation()
                //                {
                //                    Id = Guid.NewGuid(),
                //                    Title = "Danish Gambit",
                //                    Description = "Black accepts the trade offer.",
                //                    MovesToPosition = new List<Move>()
                //                    {

                //                        new Move("rnbqkbnr/pppp1ppp/8/8/3pP3/8/PPP2PPP/RNBQKBNR w KQkq - 0 3", "exd4")
                //                    },
                //                    Position = new Position()
                //                    {
                //                        Id = Guid.NewGuid(),
                //                        Move = new Move("rnbqkbnr/pppp1ppp/8/8/3pP3/2P5/PP3PPP/RNBQKBNR b KQkq - 0 3", "c3"),
                //                        Title = "Danish Gambit",
                //                        Tags = new List<string>(){"Danish Gambit", "Gambit" },
                //                        Description = "White offers Black a piece of Candy.",
                //                        Continuations = new List<Continuation>()
                //                        {
                //                            new Continuation()
                //                            {
                //                                Id = Guid.NewGuid(),
                //                                Title = "Danish Gambit",
                //                                Description = "Black accepts the candy.",
                //                                MovesToPosition = new List<Move>()
                //                                {

                //                                    new Move("rnbqkbnr/pppp1ppp/8/8/4P3/2p5/PP3PPP/RNBQKBNR w KQkq - 0 4", "dxc3")
                //                                },
                //                                Position = new Position()
                //                                {
                //                                    Id = Guid.NewGuid(),
                //                                    Move = new Move("rnbqkbnr/pppp1ppp/8/8/2B1P3/2p5/PP3PPP/RNBQK1NR b KQkq", "Bc4"),
                //                                    Title = "Danish Gambit",
                //                                    Tags = new List<string>(){ "Danish Gambit", "Gambit" },
                //                                    Description = "Delighted, White tells Black he can pet his puppy if he climbs into his van.",
                //                                    Continuations = new List<Continuation>()
                //                                    {
                //                                        new Continuation()
                //                                        {
                //                                            Id = Guid.NewGuid(),
                //                                            Title = "Danish Gambit",
                //                                            Description = "Excited by the new friendship, Black climbs inside the van.",
                //                                            MovesToPosition = new List<Move>()
                //                                            {

                //                                                new Move("rnbqkbnr/pppp1ppp/8/8/2B1P3/8/Pp3PPP/RNBQK1NR w KQkq - 0 5", "cxb2")
                //                                            },
                //                                            Position = new Position()
                //                                            {
                //                                                Id = Guid.NewGuid(),
                //                                                Move = new Move("rnbqkbnr/pppp1ppp/8/8/2B1P3/8/PB3PPP/RN1QK1NR b KQkq - 0 5", "Bxb2"),
                //                                                Title = "Danish Gambit",
                //                                                Tags = new List<string>(){ "Danish Gambit", "Gambit" },
                //                                                Description = "White closes the door poises himself to violently molest Black.",
                //                                                Continuations = new List<Continuation>()
                //                                                {
                //                                                    new Continuation()
                //                                                    {
                //                                                        Id = Guid.NewGuid(),
                //                                                        Title = "Danish Gambit",
                //                                                        Description = "Black is no stranger to sexual violence and responds sharply.",
                //                                                        MovesToPosition = new List<Move>()
                //                                                        {
                //                                                            new Move("rnbqk1nr/pppp1ppp/8/8/1bB1P3/8/PB3PPP/RN1QK1NR w KQkq - 1 6", "Bb4+")
                //                                                        },
                //                                                        Position = new Position()
                //                                                        {
                //                                                            Id = Guid.NewGuid(),
                //                                                            Move = new Move("rnbqk1nr/pppp1ppp/8/8/1bB1P3/8/PB3PPP/RN1Q1KNR b kq - 2 6", "Kf1"),
                //                                                            Title = "Danish Gambit",
                //                                                            Tags = new List<string>(){ "Danish Gambit", "Gambit" },
                //                                                            Description = "But it only serves to further arouse White, who devilishly enjoys a scuffle.",
                //                                                            Continuations = new List<Continuation>()
                //                                                        }
                //                                                    }
                //                                                }
                //                                            }
                //                                        }
                //                                    }
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    },
                //    Perspective = Color.White
                //},
                //new Study()
                //{
                //    Id = Guid.NewGuid(),
                //    Title = "Caro-Kann",
                //    Description = "The correct response to 1. e5.",
                //    Perspective = Color.Black,
                //    Continuation = new Continuation()
                //    {
                //        Id = Guid.NewGuid(),
                //        Title = "Caro-Kann",
                //        Description = "The best opening on the market.",
                //        MovesToPosition = new List<Move>()
                //        {

                //            new Move("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "-"),
                //            new Move("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", "e4"),
                //            new Move("rnbqkbnr/pp1ppppp/2p5/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2", "c6"),
                //            new Move("rnbqkbnr/pp1ppppp/2p5/8/3PP3/8/PPP2PPP/RNBQKBNR b KQkq d3 0 2", "d4")
                //        },
                //        Position = new Position()
                //        {
                //            Id = Guid.NewGuid(),
                //            Move = new Move("rnbqkbnr/pp2pppp/2p5/3p4/3PP3/8/PPP2PPP/RNBQKBNR w KQkq d6 0 3", "d5"),
                //            Title = "Caro-Kann",
                //            Tags = new List<string>() { "Caro-Kann" },
                //            Description = "Black fights for control of the center",
                //            Continuations = new List<Continuation>()
                //            {
                //                new Continuation()
                //                {
                //                    Id = Guid.NewGuid(),
                //                    Title = "Advanced Caro-Kann",
                //                    Description = "White fights for space, playing directly into Black's hand.",
                //                    MovesToPosition = new List<Move>(),
                //                    Position = new Position()
                //                    {
                //                        Id = Guid.NewGuid(),
                //                        Move = new Move("rnbqkbnr/pp2pppp/2p5/3pP3/3P4/8/PPP2PPP/RNBQKBNR b KQkq - 0 3", "e5"),
                //                        Title = "Advanced Caro-Kann",
                //                        Tags = new List<string>() { "Caro-Kann", "Advanced Caro-Kann" },
                //                        Description = "White fights for space, playing directly into Black's hand.",
                //                        Continuations = new List<Continuation>()
                //                        {
                //                            new Continuation()
                //                            {
                //                                 Title = "Advanced Caro-Kann",
                //                                 Id = Guid.NewGuid(),
                //                                Description = "The Rozman method.",
                //                                MovesToPosition = new List<Move>()
                //                                {

                //                                    new Move("rnbqkbnr/pp2pppp/8/2ppP3/3P4/8/PPP2PPP/RNBQKBNR w KQkq - 0 4", "c5"),
                //                                    new Move("rnbqkbnr/pp2pppp/8/2ppP3/3P4/2P5/PP3PPP/RNBQKBNR b KQkq - 0 4", "c3"),
                //                                    new Move("r1bqkbnr/pp2pppp/2n5/2ppP3/3P4/2P5/PP3PPP/RNBQKBNR w KQkq - 1 5", "Nc6"),
                //                                    new Move("r1bqkbnr/pp2pppp/2n5/2ppP3/3P4/2P2N2/PP3PPP/RNBQKB1R b KQkq - 2 5", "Nf3")
                //                                },
                //                                Position = new Position()
                //                                {
                //                                    Id = Guid.NewGuid(),
                //                                    Move = new Move("r2qkbnr/pp2pppp/2n5/2ppP3/3P2b1/2P2N2/PP3PPP/RNBQKB1R w KQkq - 3 6", "Bg4"),
                //                                    Title = "Advanced Caro-Kann",
                //                                    Tags = new List<string>() { "Caro-Kann", "Advanced Caro-Kann" },
                //                                    Description = "Black has successfuly employed the Rozman method. White will likely lose a pawn.",
                //                                    Continuations = new List<Continuation>()
                //                                }
                //                            },
                //                            new Continuation()
                //                            {
                //                                 Id = Guid.NewGuid(),
                //                                 Title = "Bayonett Attack",
                //                                Description = "The pro method.",
                //                                MovesToPosition = new List<Move>()
                //                                {

                //                                    new Move("rn1qkbnr/pp2pppp/2p5/3pPb2/3P4/8/PPP2PPP/RNBQKBNR w KQkq - 1 4", "Bf5"),
                //                                    new Move("rn1qkbnr/pp2pppp/2p5/3pPb2/3P2P1/8/PPP2P1P/RNBQKBNR b KQkq g3 0 4", "g4"),
                //                                    new Move("rn1qkbnr/pp2pppp/2p5/3pP3/3Pb1P1/8/PPP2P1P/RNBQKBNR w KQkq - 1 5", "Be4")
                //                                },
                //                                Position = new Position()
                //                                {
                //                                    Id = Guid.NewGuid(),
                //                                    Move = new Move("rn1qkbnr/pp2pppp/2p5/3pP3/3Pb1P1/5P2/PPP4P/RNBQKBNR b KQkq - 0 5", "f3"),
                //                                    Title = "Bayonett Attack",
                //                                    Tags = new List<string>() { "Caro-Kann", "Advanced Caro-Kann", "Bayonett Attack" },
                //                                    Description = "White sacrifices the integrity of his position to bully the bishop's nuts.",
                //                                    Continuations = new List<Continuation>()
                //                                }
                //                            }
                //                        }
                //                    }
                //                },
                //                new Continuation()
                //                {
                //                    Id = Guid.NewGuid(),
                //                    Description = "White cowars before Black, liquidating material to preserve equality for as long as possible.",
                //                    MovesToPosition = new List<Move>()
                //                    {
                //                        new Move("rnbqkbnr/pp2pppp/2p5/3P4/3P4/8/PPP2PPP/RNBQKBNR b KQkq - 0 3", "exd5")
                //                    },
                //                    Position = new Position()
                //                    {
                //                        Id = Guid.NewGuid(),
                //                        Move = new Move("rnbqkbnr/pp2pppp/8/3p4/3P4/8/PPP2PPP/RNBQKBNR w KQkq - 0 4", "cxd5"),
                //                        Title = "Exchange Caro-Kann",
                //                        Tags = new List<string>() { "Caro-Kann", "Exchange Caro-Kann" },
                //                        Description = "White cowars before Black, liquidating material to preserve equality for as long as possible.",
                //                        Continuations = new List<Continuation>()
                //                        {
                //                            new Continuation()
                //                            {
                //                                 Id = Guid.NewGuid(),
                //                                Description = "Indeed...",
                //                                MovesToPosition = new List<Move>()
                //                                {

                //                                    new Move("rnbqkbnr/pp2pppp/8/3p4/3P4/5N2/PPP2PPP/RNBQKB1R b KQkq - 1 4", "Nf3"),
                //                                    new Move("r1bqkbnr/pp2pppp/2n5/3p4/3P4/5N2/PPP2PPP/RNBQKB1R w KQkq - 2 5", "Nc6"),
                //                                    new Move("r1bqkbnr/pp2pppp/2n5/1B1p4/3P4/5N2/PPP2PPP/RNBQK2R b KQkq - 3 5", "Bb5"),
                //                                    new Move("r2qkbnr/pp2pppp/2n5/1B1p4/3P2b1/5N2/PPP2PPP/RNBQK2R w KQkq - 4 6", "Bg4"),
                //                                    new Move("r2qkbnr/pp2pppp/2n5/1B1p4/3P2b1/5N2/PPP2PPP/RNBQ1RK1 b kq - 5 6", "O-O"),
                //                                },
                //                                Position = new Position()
                //                                {
                //                                    Id = Guid.NewGuid(),
                //                                    Move = new Move("r2qkbnr/pp3ppp/2n1p3/1B1p4/3P2b1/5N2/PPP2PPP/RNBQ1RK1 w kq - 0 7", "e6"),
                //                                    Title = "Exchange Caro-Kann",
                //                                    Tags = new List<string>() { "Caro-Kann", "Exchange Caro-Kann" },
                //                                    Description = "White and Black play chess.",
                //                                    Continuations = new List<Continuation>()
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //},
                //new Study()
                //{
                //    Id = Guid.NewGuid(),
                //    Title = "Scotch",
                //    Description = "The correct response to 1. e5",
                //    Perspective = Color.White,
                //    Continuation = new Continuation()
                //    {
                //        Id = Guid.NewGuid(),
                //        Title = "Scotch",
                //        Description = "",
                //        MovesToPosition = new List<Move>()
                //        {
                //            new Move("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", "-"),
                //            new Move("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2", "-"),
                //            new Move("rnbqkbnr/pppp1ppp/8/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2", "-"),
                //            new Move("r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 2 3", "-")
                //        },
                //        Position = new Position()
                //        {
                //            Id = Guid.NewGuid(),
                //            Move = new Move("r1bqkbnr/pppp1ppp/2n5/4p3/3PP3/5N2/PPP2PPP/RNBQKB1R b KQkq d3 0 3", "-"),
                //            Title = "Scotch",
                //            Tags = new List<string>() { "Scotch" },
                //            Description = "Black is caught with his pants down after 3. d4.",
                //            Continuations = new List<Continuation>()
                //        }
                //    }
                //}
            };
    }
}
