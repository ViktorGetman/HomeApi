﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeApi.Data.Models;
using HomeApi.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace HomeApi.Data.Repos
{
    /// <summary>
    /// Репозиторий для операций с объектами типа "Room" в базе
    /// </summary>
    public class RoomRepository : IRoomRepository
    {
        private readonly HomeApiContext _context;
        
        public RoomRepository (HomeApiContext context)
        {
            _context = context;
        }

        public async Task<Room> GetRoomById(Guid id)
        {
            return await _context.Rooms.Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        ///  Найти комнату по имени
        /// </summary>
        public async Task<Room> GetRoomByName(string name)
        {
            return await _context.Rooms.Where(r => r.Name == name).FirstOrDefaultAsync();
        }

        public async Task<Room[]> GetRooms()
        {
            return await _context.Rooms.ToArrayAsync();
        }

        /// <summary>
        ///  Добавить новую комнату
        /// </summary>
        public async Task AddRoom(Room room)
        {
            var entry = _context.Entry(room);
            if (entry.State == EntityState.Detached)
                await _context.Rooms.AddAsync(room);
            
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoom(Room room, UpdateRoomQuery query)
        {
          
            // Если в запрос переданы параметры для обновления - проверяем их на null
            // И если нужно - обновляем устройство
            if (!string.IsNullOrEmpty(query.NewName))
                room.Name = query.NewName;
            if (query.NewArea != null)
                room.Area = query.NewArea.Value;
            if (query.NewGasConnected != null)
                room.GasConnected = query.NewGasConnected.Value;
            if (query.NewVoltage != null)
                room.Voltage = query.NewVoltage.Value;

            
            // Добавляем в базу 
            var entry = _context.Entry(room);
            if (entry.State == EntityState.Detached)
                _context.Rooms.Update(room);
            
            // Сохраняем изменения в базе 
            await _context.SaveChangesAsync();
        }
    }
}