using System;
using System.Linq;
using System.Threading.Tasks;
using MainData;
using MainData.Entities;

namespace Worker_Event.Services
{
    public interface IEventService
    {
        Task UpdateEventStatus();
    }

    public class EventService : IEventService
    {
        private readonly DatabaseContext _dbContext;

        public EventService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateEventStatus()
        {
            var events = _dbContext.Events
                .Where(x => !x.DeletedAt.HasValue)
                .ToList();

            foreach (var ev in events)
            {
                // Update the event status based on the current date and time
                if (CurrentDate < ev.StartDate)
                {
                    ev.Status = EventStatus.UpComing;
                }
                else if (CurrentDate >= ev.StartDate && CurrentDate < ev.EndDate)
                {
                    ev.Status = EventStatus.Happening;
                }
                else if (CurrentDate >= ev.EndDate)
                {
                    ev.Status = EventStatus.Ending;
                }

                _dbContext.Update(ev);
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private DateTime CurrentDate => DateTime.Now;
    }
}