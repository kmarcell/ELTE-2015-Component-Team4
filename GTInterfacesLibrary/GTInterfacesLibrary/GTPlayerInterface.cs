using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTInterfacesLibrary
{
    public interface GTPlayerInterface<E, P>
        where E : GTGameSpaceElementInterface
        where P : IPosition
    {
        GTPlayerInterface<E, P> playerWithRealUser(int id);
        GTPlayerInterface<E, P> playerWithAI(GTArtificialIntelligenceInterface<E, P> ai, int id);

        int id { get; set; }
		int figuresInitial { get; set; }
        int figuresRemaining {get; set;}
        int figuresLost { get; set; }

    }

	class GTPlayer<E, P> : GTPlayerInterface<E, P>
        where E : GTGameSpaceElementInterface
        where P : GTPosition
    {

        int _id;
        int _figuresRemaining;
        int _figuresLost;
		int _figuresInitial;
        GTArtificialIntelligenceInterface<E, P> ai;

        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

		public int figuresInitial {
			get {
				return _figuresInitial;
			}
			set {
				_figuresInitial = value;
			}
		}

        public int figuresRemaining
        {
            get
            {
                return _figuresRemaining;
            }
            set
            {
                _figuresRemaining = value;
            }
        }

        public int figuresLost
        {
            get
            {
                return _figuresLost;
            }
            set
            {
                _figuresLost = value;
            }
        }

        public GTPlayerInterface<E, P> playerWithRealUser(int id)
        {
            GTPlayer<E, P> player = new GTPlayer<E, P>();
            player.id = id;
            return player;
        }

        public GTPlayerInterface<E, P> playerWithAI(GTArtificialIntelligenceInterface<E, P> ai, int id)
        {
            GTPlayer<E, P> player = new GTPlayer<E, P>();
            player.id = id;
            player.ai = ai;
            return player;
        }
    }
}
