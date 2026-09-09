using EmployeeManagementApp.Data;
using EmployeeManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementApp.Repositories;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly ApplicationDbContext _context;
    public LeaveRequestRepository(ApplicationDbContext context) => _context = context;
    public async Task<IReadOnlyList<LeaveRequest>> GetAllAsync() => await _context.LeaveRequests.AsNoTracking().Include(request => request.Employee).OrderByDescending(request => request.StartDate).ToListAsync();
    public async Task<LeaveRequest?> GetByIdAsync(int id) => await _context.LeaveRequests.Include(request => request.Employee).FirstOrDefaultAsync(request => request.Id == id);
    public async Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest) { await _context.LeaveRequests.AddAsync(leaveRequest); await _context.SaveChangesAsync(); return leaveRequest; }
    public async Task UpdateAsync(LeaveRequest leaveRequest) { await _context.SaveChangesAsync(); }
    public async Task DeleteAsync(LeaveRequest leaveRequest) { _context.LeaveRequests.Remove(leaveRequest); await _context.SaveChangesAsync(); }
}