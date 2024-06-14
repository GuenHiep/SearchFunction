using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SearchFunction.Models;

namespace SearchFunction.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SearchFunctionContext _context;

        public StudentsController(SearchFunctionContext context)
        {
            _context = context;
        }

        // GET: Students
        // Phương thức để hiển thị trang danh sách sinh viên với tính năng tìm kiếm
        public async Task<IActionResult> Index(string searchString)
        {
            // Kiểm tra nếu không có dữ liệu Student trong context
            if (_context.Student == null)
            {
                return Problem("Entity set 'SearchFunctionContext.Student' is null.");
            }

            // Tạo một truy vấn ban đầu lấy tất cả sinh viên cùng với lớp học của họ
            var students = from s in _context.Student.Include(s => s.Classroom)
                           select s;

            // Nếu chuỗi tìm kiếm không rỗng hoặc không null
            if (!String.IsNullOrEmpty(searchString))
            {
                // Lọc danh sách sinh viên theo tên sinh viên hoặc tên lớp học
                students = students.Where(s => s.Name.Contains(searchString) ||
                                               s.Classroom.Name.Contains(searchString));

                // Cố gắng chuyển đổi chuỗi tìm kiếm thành giá trị GPA
                if (double.TryParse(searchString, out double gpa))
                {
                    // Nếu thành công, tiếp tục lọc danh sách theo GPA
                    students = students.Where(s => s.GPA == gpa);
                }
            }

            // Trả về view Index với danh sách sinh viên đã lọc
            return View(await students.ToListAsync());
        }

        // GET: Students/Autocomplete
        // Phương thức để cung cấp các gợi ý tìm kiếm tự động khi người dùng nhập từ khoá
        public async Task<IActionResult> Autocomplete(string term)
        {
            // Tạo một truy vấn ban đầu lấy tất cả sinh viên cùng với lớp học của họ
            var students = _context.Student.Include(s => s.Classroom).AsQueryable();

            // Nếu từ khoá tìm kiếm không rỗng hoặc không null
            if (!String.IsNullOrEmpty(term))
            {
                // Cố gắng chuyển đổi từ khoá tìm kiếm thành giá trị GPA
                double gpa;
                bool isGpa = double.TryParse(term, out gpa);

                // Lọc danh sách sinh viên theo tên sinh viên, tên lớp học hoặc GPA (nếu có)
                students = students.Where(s => s.Name.Contains(term) ||
                                               s.Classroom.Name.Contains(term) ||
                                               (isGpa && s.GPA == gpa));
            }

            // Chọn các thuộc tính cần thiết cho kết quả gợi ý và chuyển đổi thành JSON
            var result = await students
                              .Select(s => new {
                                  label = s.Name,  // Thuộc tính dùng để hiển thị trong gợi ý
                                  value = s.Name,  // Giá trị được chọn và gán cho trường nhập liệu
                                  classroom = s.Classroom.Name,  // Tên lớp học của sinh viên
                                  gpa = s.GPA  // Điểm trung bình của sinh viên
                              })
                              .ToListAsync();

            // Trả về kết quả dưới dạng JSON cho phía client (frontend)
            return Json(result);
        }



        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(s => s.Classroom)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["ClassroomId"] = new SelectList(_context.Classroom, "Id", "Name");
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,GPA,ClassroomId")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassroomId"] = new SelectList(_context.Classroom, "Id", "Name", student.ClassroomId);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["ClassroomId"] = new SelectList(_context.Classroom, "Id", "Name", student.ClassroomId);
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,GPA,ClassroomId")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassroomId"] = new SelectList(_context.Classroom, "Id", "Name", student.ClassroomId);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(s => s.Classroom)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            if (student != null)
            {
                _context.Student.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}
